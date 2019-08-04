<?php

namespace App\Http\Controllers;

use App\Providers\DatabaseProvider;
use App\Services\DatabaseService;
use App\Services\UserService;
use Illuminate\Http\Request;

class UserController extends Controller
{
    public function getLogin() {
        return view('login');
    }

    public function doLogin(UserService $db, Request $request) {

    }
}
