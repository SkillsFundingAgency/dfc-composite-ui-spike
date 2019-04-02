const config = require('./../config/config.json');

class configService {
    
    getApplication(applicationId) {
        return config.applications.find(function (x) { return x.id == applicationId; });
    }   

    getApplications() {
        return config.applications;
    }  

    getGoogle() {
        return config.google;
    }  

    getShell() {
        return {
            "id": 100,
            "description": "Default",
            "layout": "_layoutWithSideBar.raz",
            "branding": "dark"
        };
    }  
}

module.exports = configService;