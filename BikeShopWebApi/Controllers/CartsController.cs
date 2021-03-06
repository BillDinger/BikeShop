﻿using System;
using System.Web;
using System.Web.Http;
using BikeShopWebApi.CommerceService;
using BikeShopWebApi.ProductService.Models;
using Castle.Core.Logging;

namespace BikeShopWebApi.Controllers
{
    [Route("api/v1/cart")]
    public class CartsController : ApiController
    {

        private ILogger Logger { get; }

        private ICommerceService CommerceService { get; }

        private HttpContextBase Context { get; }

        public CartsController(ICommerceService commerceService, 
            ILogger logger, 
            HttpContextBase context)
        {
            if (commerceService == null)
            {
                throw new ArgumentNullException(nameof(commerceService));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            CommerceService = commerceService;
            Logger = logger;
            Context = context;
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            if (Guid.Empty == id)
            {
                return BadRequest();
            }

            try
            {
                return Ok(CommerceService.Get(id));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {nameof(CartsController)} in method {nameof(Get)}", ex);
                return InternalServerError();
            }

        }

        [HttpPatch]
        public IHttpActionResult Add([FromBody]Product product, Guid id)
        {
            if (Guid.Empty == id)
            {
                return BadRequest();
            }
            if (product == null)
            {
                return BadRequest();
            }

            try
            {
                CommerceService.Add(product, id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {nameof(CartsController)} in method {nameof(Add)}", ex);
                return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete( Guid id)
        {
            if (Guid.Empty == id)
            {
                return BadRequest();
            }

            try
            {
                CommerceService.Remove(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {nameof(CartsController)} in method {nameof(Delete)}", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult Post(Guid id)
        {
            if (Guid.Empty == id)
            {
                return BadRequest();
            }


            try
            {
                if (Context.Session != null)
                {
                    var sessionId = Context.Session.SessionID;
                    Logger.Info($"Customer {sessionId} purchasing cart {id}");
                }
                CommerceService.Purchase(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {nameof(CartsController)} in method {nameof(Add)}", ex);
                return InternalServerError();
            }
        }


    }
}