<header class="main-heading">
    <h1 class="container">Blog</h1>
</header>

<div class="content-section style-default">
    <div class="container">
        <p><em class="big">Welcome to my blog,</em> where I post interesting thoughts on web development and programming, demos and examples of code, and rants about the internet and software.</p>

        <?php if(count($articles) < 1): ?>
            <p>No articles found!</p>
        <?php else: foreach($articles as $article): ?>
            <article>
                <h2><a href="<?php echo $article->getUrl(); ?>"><?php echo $article->getTitle(); ?></a></h2>
                <p><?php echo $article->getSummary(250); ?>...</p>
                <p><a href="<?php echo $article->getUrl(); ?>">Read full post</a></p>
            </article>
        <?php endforeach; endif; ?>
    </div>
</div>