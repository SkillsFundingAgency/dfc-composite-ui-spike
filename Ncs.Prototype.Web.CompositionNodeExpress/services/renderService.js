process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const axios = require('axios');
const querystring = require('querystring');
const jwt = require('jsonwebtoken');
const userService = require('./userService');
const configService = require('./configService');

class renderService {

    constructor() {
        this.userService = new userService();
    }

    async renderGet(req, applicationOwnerId, url) {
        try {
            var token = this.generateAccessToken(this.userService.getCurrentUser(req));
            var response = await axios.get(
                url,
                {
                    headers: {
                        "Authorization": 'Bearer ' + token,
                        "X-Correlation-ID": req.correlationId
                    }
                });
            var data = response.data;
            data = this.process(applicationOwnerId, data);
            return data;
        } catch (err) {
            return "Service unavailable";
        }
    }

    async renderPost(req, applicationOwnerId, url, data) {
        try {
            var token = this.generateAccessToken(this.userService.getCurrentUser(req));
            var response = await axios.post(
                url,
                querystring.stringify(data),
                {
                    headers: {
                        "Authorization": "Bearer " + token,
                        "X-Correlation-ID": req.correlationId
                    }
                });
            var responseData = response.data;
            data = this.process(applicationOwnerId, responseData);
            return data;
        } catch (err) {
            return "Service unavailable";
        }
    }

    process(applicationOwnerId, response) {
        response = this.rewriteUrls(applicationOwnerId, response);
        return response;
    }

    rewriteUrls(applicationOwnerId, response) {
        var attributesNames = ['href', 'action'];
        var quoteChars = ['"', "'"];

        attributesNames.forEach(function (attributeName) {
            quoteChars.forEach(function (quoteChar) {
                var searchFor = attributeName + "=" + quoteChar;
                var replaceWith = attributeName + "=" + quoteChar + "/application/renderView?applicationId=" + applicationOwnerId + "&data=";
                response = response.replace(new RegExp(searchFor, 'g'), replaceWith);
            });
        });

        return response;
    }

    generateAccessToken(user) {
        var configSvc = new configService();
        var configGoogle = configSvc.getGoogle();
        const token = jwt.sign(
            user,
            configGoogle.clientSecret,
            {
                expiresIn: configGoogle.expiresIn,
                audience: configGoogle.clientId,
                issuer: configGoogle.issuer,
                subject: user.id || "0"
            });

        return token;
    }
}

module.exports = renderService;