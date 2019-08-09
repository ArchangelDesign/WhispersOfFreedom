@include('header-normal')


<div class="container">
    <div class="row">
        <div class="col-xl-4 col-md-6 col-lg-6 col-sm-8 col-xs-14 col-xl-offset-4 col-lg-offset-3 col-md-offset-3 col-sm-offset-2">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Registration</h4>
                </div>
                <div class="panel-body">
                    <form method="post" action="{{ route('do-register') }}" onsubmit="return isValid()">
                        @csrf
                        @if($errors->any())
                            <div class="alert alert-danger">
                                <h4>Errors</h4>
                                @foreach($errors->all() as $k => $error)
                                    <li>{{$error}}</li>
                                @endforeach
                            </div>
                        @endif
                        <div class="form-group @if ($errors->has('username')) {{'has-error'}} @endif">
                            <label for="username" class="control-label">Username</label>
                            <input id="username" name="username" type="text" class="form-control"
                                   placeholder="Username">
                        </div>
                        <div class="form-group @if ($errors->has('email')) {{'has-error'}} @endif">
                            <label for="email" class="control-label">Email address</label>
                            <input id="email" name="email" type="email" class="form-control" placeholder="Email address"
                                   aria-describedby="basic-addon1">
                        </div>
                        <div class="form-group @if ($errors->has('password')) {{'has-error'}} @endif">
                            <label for="password" class="control-label">Password</label>
                            <input id="password" name="password" type="password" class="form-control">
                        </div>
                        <div class="form-group">
                            <label for="password-repeat" class="control-label">Re-type password</label>
                            <input id="password-repeat" type="text" class="form-control">
                        </div>
                        <div class="form-group">

                            <button type="submit" class="btn btn-info">Register</button>

                        </div>

                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
@include('footer-normal')

<script>
    function isValid() {
        return true;
    }
</script>