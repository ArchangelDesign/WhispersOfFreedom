<h4>Welcome</h4>

<form method="post" action="{{ route('do-login') }}">
@csrf
    <input name="username" type="text">
    <input name="password" type="password">
    <input type="submit" value="Login">
</form>