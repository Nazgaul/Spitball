using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using System.Reflection;
using System.Configuration;
using Autofac;
using Cloudents.Core;
using Cloudents.Infrastructure;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3.Accounts;

namespace Cloudents.Web.Controllers
{
    public class BlockchainController : Controller
    {
        private readonly IBlockchainProvider _blockchainProvider;

        private List<KeyValuePair<string, string>> _account = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", "0x27e739f9dF8135fD1946b0b5584BcE49E22000af"), // Hadar
            new KeyValuePair<string, string>("0b791c5dbc5704a075e3af8c654238e5eac78d2ac5dd3a7cd4c5a92a2e578f46", "0xF8a5AD3Bc74196d759f7E021738AeC542a69Bc16"), // Ubuntu
            new KeyValuePair<string, string>("41e89afc7a3cd97716f44a5763c6b39a601358f4a0f521baf1039255ab5acf41", "0xF20E95b9E0507dE2F52500F03a377d79aF55d7f0") //Ram

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

        public async Task<IActionResult> TransferMoney([FromForm]Model model)
        {
            string from = _account[model.fromPK].Key;
            string to = _account[model.toAddress].Value;
            var transactionReceipt = await _blockchainProvider.TransferMoneyAsync(from, to, model.amount);

            return Ok(transactionReceipt);
        }

        public class Model

        {
            public int fromPK { get; set; }
            public int toAddress { get; set; }
            public float amount { get; set; }
        }

        [HttpGet]
        public async Task<BigInteger> GetBalance(int fromPK)
        {
            string from = _account[fromPK].Key;
            BigInteger balance = await _blockchainProvider.GetTokenBalanceAsync(from);
            return balance;
        }

        [HttpPost]
        public string CreateAccount()
        {
            var account = _blockchainProvider.CreateAccount();
            return account.Address;
        }

        [HttpPost]
        public async Task<bool> SetInitialBalance(string address)
        {
            bool result = await _blockchainProvider.SetInitialBalance(address);
            return result;
        }
    }
}
