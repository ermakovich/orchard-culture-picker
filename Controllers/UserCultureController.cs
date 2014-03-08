﻿using System;
using System.Web;
using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.CulturePicker.Services;
using Orchard.Localization;
using Orchard.Mvc.Extensions;

namespace Orchard.CulturePicker.Controllers {
    public class UserCultureController : Controller {
        private readonly ILocalizableContentService _localizableContentService;

        public UserCultureController(IOrchardServices services, ILocalizableContentService localizableContentService) {
            Services = services;
            _localizableContentService = localizableContentService;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult ChangeCulture(string cultureName) {
            if (string.IsNullOrEmpty(cultureName)) {
                throw new ArgumentNullException(cultureName);
            }

            string returnUrl = Utils.GetReturnUrl(Services.WorkContext.HttpContext.Request);

            AutoroutePart currentRoutePart;
            //returnUrl may not correspond to any content and we use "Try" approach
            if (_localizableContentService.TryGetRouteForUrl(returnUrl, out currentRoutePart)) {
                AutoroutePart localizedRoutePart;
                //content may not have localized version and we use "Try" approach
                if (_localizableContentService.TryFindLocalizedRoute(currentRoutePart.ContentItem, cultureName, out localizedRoutePart)) {
                    returnUrl = localizedRoutePart.Path;
                }
            }

            if (!returnUrl.StartsWith("~/")) {
                returnUrl = "~/" + returnUrl;
            }

            return this.RedirectLocal(returnUrl);
        }
    }
}