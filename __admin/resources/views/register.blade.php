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
                                   placeholder="Username" required>
                        </div>
                        <div class="form-group @if ($errors->has('email')) {{'has-error'}} @endif">
                            <label for="email" class="control-label">Email address</label>
                            <input id="email" name="email" type="email" class="form-control" placeholder="Email address"
                                   aria-describedby="basic-addon1" required>
                        </div>
                        <div class="form-group @if ($errors->has('password')) {{'has-error'}} @endif">
                            <label for="password" class="control-label">Password</label>
                            <input id="password" name="password" type="password" class="form-control" required>
                        </div>
                        <div class="form-group">
                            <label for="password-repeat" class="control-label">Re-type password</label>
                            <input id="password-repeat" type="password" class="form-control" required>
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

<div class="modal fade" id="validation-error" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="myModalLabel">Validation error</h4>
            </div>
            <div class="modal-body">
                <p id="validation-error-message"></p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<script>
    function isValid() {
        let password1 = $("#password").val();
        let password2 = $("#password-repeat").val();
        let passwordValidator = new RegExp('(?=.{8,})(?=.*[0-9])(?=.*[A-Z])');
        if (password1.length < 8)
            return validationError("Password needs to be at least 8 characters long.");
        if (password1 !== password2)
            return validationError("Entered passwords are not the same");
        if (!passwordValidator.test(password1))
            return validationError("Password must contain at least 1 number and one capital and lowercase letter");

        return true;
    }

    function validationError(message) {
        $("#validation-error-message").text(message);
        $("#validation-error").modal();
        return false;
    }
</script>