@include('header-normal')

<div class="container">

    <div class="jumbotron">
        <h2>Whispers of Freedom
            <small>Operation Tempest</small>
        </h2>
        <media>
            <div class="media-left">

                <img style="width: 120px" class="media-object" src="/img/lokajski_poczta_glowna_1944_.jpg">
            </div>
            <div class="media-body">
                <p>In the summer of 1944, Polish underground resistance, led by the Home Army,
                    attempted to liberate Warsaw from German occupation.
                    WoF is attempting to replicate that experience by allowing you to command armed forces on the
                    streets of 1944 Warsaw in an online tactical simulation.
                    Join opensource development on GitHub or beta-testing.</p>
            </div>
        </media>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <p>Sign up for a newsletter</p>
                </div>
                <div class="panel-body">
                    <div class="input-group">
                        @csrf
                        <input id="newsletter-email" type="email" class="form-control" placeholder="email address">
                        <span class="input-group-btn">
                            <button id="newsletter-signup-button" class="btn btn-default" type="button">Go!</button>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="row">

        <div class="col-lg-4">

            <div class="thumbnail">
                <div class="embed-responsive embed-responsive-16by9">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/mTUgrtn2mCE"
                            frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>
                <div class="caption">
                    <h4>Wof #12</h4>
                    <p>
                        Everybody knows that Unity framework is not thread-safe, but does that mean we need to handle
                        everything in one thread and use yielding for concurrency?
                        Of course not, every game needs to utilize multithreading especially an online game like WoF.
                        The topic of multithreading will be returning constantly throughout the process but now we have
                        a working solution, kinda POC of handling multithreading.
                    </p>
                </div>

            </div>
        </div>

        <div class="col-lg-4">
            <div class="thumbnail">
                <div class="embed-responsive embed-responsive-16by9">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/j8pdJHeojP4" frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>

                <div class="caption">
                    <h4>Wof #11</h4>
                    <p>
                        Finally, I have released a public server hosted on DigitalOcean, that will allow us to conduct
                        the first concurrency test. For that reason, I've replaced HashMaps with Hashtables and
                        meanwhile discovered a bug with TCP handshake, now the client properly establishes TCP
                        connection after starting a session and only one session is allowed from each client account.
                        Hopefully, the next step will be releasing a lobby screen with chat functionality so we can
                        actually test this architecture which is already showing many fundamental problems with it.
                    </p>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="thumbnail">
                <div class="embed-responsive embed-responsive-16by9">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/nExLEaLKaTI" frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>

                <div class="caption">
                    <h4>Wof #10</h4>
                    <p>
                        It seems that every time I want to move on to lobby screen there is something on the main menu
                        screen not letting me do that.
                        At first, I wanted a simple notification in the corner of the screen, plain text, fade in and
                        out, however, once I looked at that I came t the conclusion that it just plain didn't work, not
                        only it was bearly visible but there was an issue when multiple notifications were displayed.
                        Since it is the only way to notify the user of any errors I had to fix that before moving on
                        with anything.
                    </p>
                </div>
            </div>
        </div>


    </div>
</div>

<div class="modal fade" id="confirmation" tabindex="-1" role="dialog" aria-labelledby="confirmation-label">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span>
                </button>
                <h4 class="modal-title" id="confirmation-label">Subscription confirmed</h4>
            </div>
            <div class="modal-body">
                <p id="validation-error-message">Thank you for your interest. Your subscription has been confirmed.
                    From time to time we will send you an update on progress and eventually an invitation to beta
                    testing.</p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
            </div>
        </div>
    </div>
</div>

@include('footer-normal')

<script>
    $(function () {
        $("#newsletter-signup-button").click(signup);
    });

    function signup() {
        let email = $("#newsletter-email").val();
        let token = $('[name="_token"]').val();
        let regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            alert('Invalid email address.');
            return;
        }
        $.ajax({
            type: "POST",
            url: "/user/newsletter/signup",
            data: {'email': email, '_token': token},
            success: function (res) {
                let result = res.result;
                let message = res.message;
                if (result) {
                    $("#newsletter-email").val('');
                    $("#confirmation").modal();
                } else {
                    alert("Something went wrong: " + message);
                }
            }
        });
    }
</script>