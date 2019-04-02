const express = require('express');
const uuid = require('uuid/v4');

module.exports = {
    correlationIdProvider: function (req, res, next) {
        req.correlationId = uuid();
        next();
    }
}