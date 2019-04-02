const configService = require('./configService');
const renderService = require('./renderService');
const applicationModel = require('./../models/application');

class applicationService {

    constructor() {
        this.configService = new configService();
        this.renderService = new renderService();
    }

    getApplication(applicationId) {
        var configApp = this.configService.getApplication(applicationId);
        var modelApp = this.getApplicationModel(configApp);
        return modelApp;
    }

    async getEntryMarkup(req, applicationId) {
        var applicationModel = this.getApplication(applicationId);
        applicationModel.applicationHtml = await this.renderService.renderGet(req, applicationId, applicationModel.entrypointUrl);
        await this.loadRegions(req, applicationModel);
        return applicationModel;
    }

    async getApplicationMarkup(req, applicationId, view) {
        var applicationModel = this.getApplication(applicationId);
        applicationModel.applicationHtml = await this.renderService.renderGet(req, applicationId, applicationModel.rootUrl + view);
        await this.loadRegions(req, applicationModel);
        return applicationModel;
    }

    async postApplicationMarkup(req, applicationId, view, data) {
        var applicationModel = this.getApplication(applicationId);
        applicationModel.applicationHtml = await this.renderService.renderPost(req, applicationId, applicationModel.rootUrl + view, data);
        await this.loadRegions(req, applicationModel);
        return applicationModel;
    }

    async loadRegions(req, applicationModel) {
        applicationModel.appNavBarHtml = await this.renderService.renderGet(req, applicationModel.applicationId, applicationModel.appNavBarUrl);
    }

    getApplicationModel(source) {
        var result = new applicationModel();
        result.title = source.title;
        result.description = source.description;
        result.layout = source.layout;
        result.mainMenuText = source.mainMenuText;
        result.routeName = source.routeName;
        result.showSideBar = source.showSideBar;
        result.requiresAuthorization = source.requiresAuthorization;
        result.rootUrl = source.rootUrl;
        result.heakthCheckUrl = source.heakthCheckUrl;
        result.entrypointUrl = source.entrypointUrl;
        result.sidebarUrl = source.sidebarUrl;
        result.appNavBarUrl = source.appNavBarUrl;
        result.breadcrumbsUrl = source.breadcrumbsUrl;
        result.personalisationUrl = source.personalisationUrl;
        result.backButtonUrl = source.backButtonUrl;
        result.branding = source.branding;
        return result;
    }
}

module.exports = applicationService;