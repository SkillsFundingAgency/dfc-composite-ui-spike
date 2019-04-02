'use strict';
const express = require('express');
const router = express.Router();
const pageViewModel = require('./../viewModels/pageViewModel');

router.get('/', function (req, res) {
    var vm = getPageViewModel();
    res.render('application/render', vm);
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
