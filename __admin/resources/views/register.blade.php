<h4>Registration</h4>

<form method="post" action="{{ route('do-register') }}">
    @csrf
    @if($errors->any())
        <h1>Errors</h1>
        @foreach($errors->all() as $error)
            <li>{{$error}}</li>
        @endforeach
    @endif
    <input name="email" type="text">
    <input name="username" type="text">
    <input name="password" type="password">
    <input type="submit" value="Login">
</form>