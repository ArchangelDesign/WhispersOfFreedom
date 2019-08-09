@include('header-normal')

<div class="container">

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

@include('footer-normal')