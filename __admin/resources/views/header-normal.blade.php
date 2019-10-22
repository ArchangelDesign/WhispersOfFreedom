<!doctype html>
<html class="no-js" lang="">

<head>
    <meta charset="utf-8">
    <title>Whispers of Freedom</title>
    <meta name="description" content="Online tactical game">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link rel="manifest" href="/site.webmanifest">
    <link rel="apple-touch-icon" href="/icon.png">
    <!-- Place favicon.ico in the root directory -->

    <link rel="stylesheet" href="/css/normalize.css">
    <link rel="stylesheet" href="/css/main.css">
    <link rel="stylesheet" href="/css/bootstrap.css">
    <link rel="stylesheet" href="/css/bootstrap-theme.css">
    <link rel="stylesheet" href="/css/app.css">

    <meta name="theme-color" content="#fafafa">
</head>

<body>
<!--[if IE]>
<p class="browserupgrade">You are using an <strong>outdated</strong> browser. Please <a href="https://browsehappy.com/">upgrade
    your browser</a> to improve your experience and security.</p>
<![endif]-->


<nav class="navbar navbar-static-top">
    <div class="container-fluid">
        <!-- Brand and toggle get grouped for better mobile display -->
        <div class="navbar-header">
            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse"
                    data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="{{route('home')}}">
                <img src="/img/logo-bw.png" class="img-circle" style="height: 30px;">
            </a>
        </div>

        <!-- Collect the nav links, forms, and other content for toggling -->
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
            <ul class="nav navbar-nav">
                <li class="active">
                    <a href="/">Home <span class="sr-only">(current)</span></a>
                </li>
                <li>
                    <a href="https://github.com/ArchangelDesign/WhispersOfFreedom">
                        <img style="height: 25px" src="/img/github-icon.svg">&nbsp;&nbsp;GitHub repository</a>
                </li>
                <li><a href="https://www.youtube.com/playlist?list=PLbaAAg7kQaeGxsT3mtIoBC3eHiZrf86BW">
                        <img style="height: 25px" src="/img/youtube_64dp.png">&nbsp;&nbsp;YouTube live
                        development</a>
                </li>
                <li><a href="https://trello.com/b/umU4JxFI/whispers-of-freedom">
                        <img style="height: 25px" src="/img/trello-icon.png">&nbsp;&nbsp;Trello Board</a></li>
            </ul>
            <ul class="nav navbar-nav navbar-right">
                <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true"
                       aria-expanded="false">Account <span class="caret"></span></a>
                    <ul class="dropdown-menu">
                        @if(session('logged_in'))
                            <li><a href="#">My profile</a></li>
                        @else
                            <li><a href="{{route('login')}}">Login</a></li>
                            <li><a href="{{route('register')}}">Register</a></li>
                            <li role="separator" class="divider"></li>
                            <li><a href="#">Separated link</a></li>
                        @endif
                    </ul>
                </li>
            </ul>
        </div><!-- /.navbar-collapse -->
    </div><!-- /.container-fluid -->
</nav>