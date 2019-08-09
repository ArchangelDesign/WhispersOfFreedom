@include('header-normal')

<div class="container">
    <div class="row">
        <div class="col-xl-4 col-md-6 col-lg-6 col-sm-8 col-xs-14 col-xl-offset-4 col-lg-offset-3 col-md-offset-3 col-sm-offset-2">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Login</h4>
                </div>
                <div class="panel-body">

                    <form method="post" action="{{ route('do-login') }}">
                        @csrf
                        <div class="form-group">
                            <label for="username">Username or email</label>
                            <input class="form-control" id="username" name="username" type="text">
                        </div>
                        <div class="form-group">
                            <label for="password">Password</label>
                            <input class="form-control" id="password" name="password" type="password">
                        </div>

                        <div class="form-group">
                            <input class="btn btn-primary pull-right" type="submit" value="Login">
                        </div>
                    </form>

                </div>
            </div>
        </div>
    </div>
</div>

@include('footer-normal')