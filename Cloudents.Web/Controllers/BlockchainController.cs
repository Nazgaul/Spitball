using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Controllers
{
    public class BlockchainController : Controller
    {
        private readonly IBlockchainProvider _blockchainProvider;

        private List<KeyValuePair<string, string>> _account = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("private", "public")
        };

        public BlockchainController(IBlockchainProvider blockchainProvider)
        {
            _blockchainProvider = blockchainProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TransferMoney()
        {
            return Ok();

        }

        //public class BlockChainModel
        //{
        //    public string 
        //}
    }
}
