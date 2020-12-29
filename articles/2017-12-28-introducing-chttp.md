+++
title = "Introducing cHTTP; or, Why Pure Rust Is Not A Worthy Goal"
author = "Stephen Coakley"
date = "2017-12-28"
tags = ["rust", "chttp", "isahc"]
+++

Friends, today I have two messages to share with you. The first is to introduce to you a new library for the Rust language that I have poured some of my tea-powered energy into, so that you may be informed of its purpose and design. The second is much more significant; that is, to discuss an attitude I have observed _in general_ amongst the Rust community.

First, let me introduce to you [cHTTP]: a practical HTTP client for Rust. While the working description is, "The practical HTTP client that is fun to use", I do not wish you to take offense. I am not calling other HTTP clients in the Rust ecosystem _impractical_; the primary goal of cHTTP that should underline every design choice is practicality. As of this writing, the current version is only `0.1.x`, so some design is subject to change as more work is done on the library.

What makes cHTTP practical? The obvious choice I made here is to use [libcurl] underneath the hood instead of using a pure Rust library like [Hyper]. I'll talk about some of the reasonings and implications behind this in a bit, but it really boils down to stability and features. libcurl is used by an enormous number of applications on many different platforms, and has been well tested and supported. Since libcurl is so great, I decided to wrap it in a nice, Rustic API that abstracts away some of libcurl's peculiarities and integrates it more with the Rust ecosystem.

The two things that cHTTP brings to the table that plain libcurl doesn't offer is integration with the budding [http] library, and streaming bodies that make use of the standard [`Read`](https://doc.rust-lang.org/std/io/trait.Read.html) trait. Offering a streaming API is always a bit tricky, since you have to let the response body "borrow" the connection socket somehow so that you can read the body directly off of the socket. There's lots of different approaches to handling this. For example, Apache's robust [`HttpClient`](https://hc.apache.org/httpcomponents-client-ga/) Java library uses a connection pooling approach. When you execute a request, the response takes control of the connection that it originated from. Closing the response entity returns the connection back to the pool.

cHTTP takes a similar approach to Apache's design. A `Client` actually holds a pool of libcurl handles. When making a request, the handle used is wrapped in a special `Stream` struct and returned as the response body. In order to facilitate a "pull"-based API, each curl "easy" handle has its own "multi" handle, which allows every `read` on the response to call `curl_multi_perform()` as necessary on just that easy handle. This still isn't quite ideal, as we lose libcurl's own smart connection pooling which requires using just one multi handle. Sharing a multi handle would require either using a mutex, or losing `Send` on the response.

Some other things I incorporated into the initial design:

- Simple API for the simple use case: Just call `chttp::get()` for a one-off `GET` request.
- No requirement for background threads: it is your choice to use multiple threads or not.
- Control over response buffering: You can consume response data at your own pace and choose whether to keep it in memory, write it to disk, or discard it.

If you are interested in seeing more of the API, you can check out [the documentation](https://docs.rs/chttp) if you like.

----

Now, I know what you're thinking. You're thinking, "What's the big deal with using Hyper?" That gets me into the second part of this post, in which I will say this:

> Writing applications or libraries in pure Rust, in and of itself, is not a worthy goal.

This is unlikely to be a highly controversial statement. In fact, many of you reading may even be nodding your heads in agreement. Still, while I do not often see verbal disagreement, I do get the general sense amongst the community that a "pure Rust" solution is inherently better than, well, a non-"pure Rust" solution. Allow me to elaborate.

I concur that having a Rust-only codebase can lend itself certain advantages, which may include: less tooling required, easier package management, less runtime dependencies, or less `unsafe` blocks. If these were the advantages, and a mixed language solution offered _no_ advantages, then I _would_ say that pure Rust is always better. But that is rarely the case. When considering using some non-Rust library (usually C) in your Rust code, you have to weigh the benefits of pure Rust against the benefits that the library offers you. Sometimes it is worth using the library, sometimes it is not. There is no universally correct answer here; the right answer has to be chosen in a case-by-case manner.

One reason why libcurl is so popular is because of the trust it has earned with developers and businesses. It takes a lot more than some unit tests to prove reliability and earn trust. libcurl has been around for a long time, and is still actively developed and improved. There are other quality libraries out there as well that have stood the test of time. The language they were written in is not their primary selling point; the language is more about longevity and speaks to the time they were created in. The selling point is the code's quality, features, or reliability.

All I really ask is that we properly weigh the benefits and downsides of using external libraries. If there's a well-used, reliable C library out there that can solve your problem, then use it! I know that Rust offers a lot of benefits for program safety and correctness, but it isn't always necessary to rewrite a solution in Rust. I'm also not saying we shouldn't try to rewrite things, that's the fun of experimenting! Who knows, maybe something even better than existing solutions will come from it.

_Note: Typically we talk about libraries written in C, and while C is undoubtedly the most prevalent language that most "native" libraries are written in, technically any language that can expose a C ABI can be used as well (like C++ or D)._


[cHTTP]: https://github.com/sagebind/chttp
[http]: https://github.com/hyperium/http
[Hyper]: https://hyper.rs
[libcurl]: https://curl.haxx.se/libcurl/
