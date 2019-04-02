'use strict';
const express = require('express');
const router = express.Router();
const pageViewModel = require('./../viewModels/pageViewModel');

router.get('/login', function (req, res) {
    var vm = getPageViewModel();
    res.render('account/login', vm);    
});

router.get('/logout', function (req, res) {
    req.logout();
    res.redirect('/');
});

function getPageViewModel() {
    var vm = new pageViewModel();
    vm.title = 'Default Title';
    vm.description = 'Default description';
    vm.layout = '_layoutWithSideBar';
    vm.showSideBar = false;
    vm.branding = 'dark';
    return vm;
}

module.exports = router;
