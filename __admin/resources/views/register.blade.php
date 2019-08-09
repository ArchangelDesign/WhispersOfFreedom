@include('header-normal')
<h4>Registration</h4>

<form method="post" action="{{ route('do-register') }}" onsubmit="return isValid()" class="">
    @csrf
    @if($errors->any())
        <h1>Errors</h1>
        @foreach($errors->all() as $error)
            <li>{{$error}}</li>
        @endforeach
    @endif
    <div class="input-group">
        <label for="username">Username</label>
        <input id="username" name="username" type="text" class="form-control" placeholder="Username">
    </div>
    <div class="input-group">
        <label for="email">Email address</label>
        <input id="email" name="email" type="email" class="form-control" placeholder="Email address" aria-describedby="basic-addon1">
    </div>
    <div class="input-group">
        <label for="password">Password</label>
        <input id="password" name="password" type="password" class="form-control">
    </div>
    <div class="input-group">
        <label for="password-repeat">Re-type password</label>
        <input id="password-repeat" type="text" class="form-control">
    </div>
    <input type="submit" value="Register" class="btn btn-info">
</form>

@include('footer-normal')

<script>
    function isValid() {
        return false;
    }
</script>