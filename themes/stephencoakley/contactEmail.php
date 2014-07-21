<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="utf-8">
        <title><?=$subject?></title>

        <style>
            body {
                max-width: 48em;
                margin: auto;
                padding: 2em;
                font-size: 1em;
                font-family: "Calibri", "Helvetica", sans-serif;
            }

            h1, h2 {
                font-family: "Cambria", serif;
            }
        </style>
    </head>

    <body>
        <main>
            <h1><?=$subject?></h1>
            <?=$content?>
        </main>
    </body>
</html>