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
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/Bz2oXcCudNk" frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>
                <div class="caption">
                    <h4>Wof #9</h4>
                    <p>
                        Some cleanup was required, heartbeat was coming even after the client has disconnected. Now
                        along with heartbeat we're sending some stats about the load of the server. That will be crucial
                        for live testing, hopefully next month I'll be uploading the binary to the server but first we
                        need CI to be set up.
                    </p>
                </div>

            </div>
        </div>


        <div class="col-lg-4">
            <div class="thumbnail">
                <div class="embed-responsive embed-responsive-16by9">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/lAWZ3nFe5Io" frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>

                <div class="caption">
                    <h4>Wof #8</h4>
                    <p>
                        First major functionality is starting to come together. Some new problems have occurred but
                        nothing seriously holding me back. If everything goes well, in the next video I will finish
                        Lobby on the client side, listing connected clients and allowing to exit lobby and start combat.
                    </p>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="thumbnail">
                <div class="embed-responsive embed-responsive-16by9">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/SesLqs4Jo9A" frameborder="0"
                            allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
                            allowfullscreen></iframe>
                </div>

                <div class="caption">
                    <h4>Wof #7</h4>
                    <p>
                        Working on battle list in the lobby and implementing fixes to TCP/IP server.
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
                From time to time we will send you and update on progress and eventually an invitation to beta testing.</p>
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