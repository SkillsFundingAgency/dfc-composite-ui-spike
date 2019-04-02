'use strict';
var express = require('express');
var router = express.Router();
var applicationService = require('./../services/applicationService');
var pageViewModel = require('./../viewModels/pageViewModel');

router.get('/renderEntryPoint/:applicationId', async function (req, res) {
    var applicationId = req.params.applicationId;
    var appService = new applicationService();
    var applicationModel = await appService.getEntryMarkup(req, applicationId);
    var pageVm = getPageViewModel(applicationModel);
    res.render('application/render', pageVm);
});

router.get('/renderView', async function (req, res) {
    var applicationId = req.query.applicationId;
    var view = req.query.data;
    var appService = new applicationService();
    var applicationModel = await appService.getApplicationMarkup(req, applicationId, view);
    var pageVm = getPageViewModel(applicationModel);
    res.render('application/render', pageVm);
});

router.post('/renderView', async function (req, res) {
    var applicationId = req.query.applicationId;
    var view = req.query.data;
    var appService = new applicationService();
    var applicationModel = await appService.postApplicationMarkup(req, applicationId, view, req.body);
    var pageVm = getPageViewModel(applicationModel);
    res.render('application/render', pageVm);
});

function getPageViewModel(application) {
    var vm = new pageViewModel();
    vm.title = application.title;
    vm.description = application.description;
    vm.layout = application.layout;
    vm.showSideBar = application.showSideBar;
    vm.branding = application.branding;
    vm.applicationHtml = application.applicationHtml;
    vm.appNavBarHtml = application.appNavBarHtml;
    return vm;
}

module.exports = router;
