# dfc-composite-ui-spike

This project provides a Proof of Concept for using a Composite UI (Shell application) to dynamically output markup from a number of external application sources.

The intention was to show that a number of external web applications can be aggregated into a composite UI, with personalisation for signed in users, utilizing Single Sign On across all the applications (This PoC authenticates to Google accounts).

The PoC also demonstrates the following:

* Healthchecks
* Dynamic registration of child applications
* Dynamic changes to branding (by changes to application registration)
* Dynamic changes to application status (online/offline/healthy etc)
* Personalisation
* Circuit Breaker pattern
* Retry pattern
* Typed Http client
* Polly pattern
* Global sitemap (aggregated from child applications)

## Getting Started

This is a self-contained Visual Studio 2017 solution containing a number of projects (web applications and web API).

### Prerequisites

Microsoft Visual Studio 2017 with .Net core 2.2

### Installing

Clone the project and open the solution in Visual Studio 2017.

To run the project, use "Start startup projects" and select multiple startup projects, then select all web applications (Project names pefixed with "Ncs.Prototype.Web").

Then run the solution.

Once running, browse to main entrypoint which is the "Ncs.Prototype.Web.Composition" project. From here, you can navigate to the other applications which load into regions of the page in the composite UI.

## Deployments

This is a Proof of Concept solution and if not intended for production deployment.

## Built With

* Microsoft Visual Studio 2017
* .Net Core 2.2

## Authors

* **Ilyas Dhin** - *Initial work* 
* **Ian Crisp** - *Initial work* 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Wayne Busby
