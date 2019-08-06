<?php

namespace App\Http\Controllers;

use App\Providers\DatabaseProvider;
use App\Services\DatabaseService;
use App\Services\UserService;
use Illuminate\Http\Request;

class UserController extends Controller
{
    public function getLogin()
    {
        return view('login');
    }

    public function doLogin(UserService $db, Request $request)
    {

//        $db->registerUser('');
//        $db->fetchUserByEmail('');
    }

    public function doRegister(UserService $userService, Request $request)
    {
        $validated = $request->validate([
            'email' => 'required|email',
            'password' => 'required|regex:/(?=.{8,})(?=.*[0-9])(?=.*[A-Z])/i',
            'username' => 'required|length'
        ]);

        $username = $validated['username'];
        $password = $validated['password'];
        $email = $validated['email'];

        $userService->registerUser($email, $username, $password);
    }
}
