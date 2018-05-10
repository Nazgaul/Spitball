﻿// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Mail;
using Cloudents.Infrastructure.Search.Tutor;
using Cloudents.Infrastructure.Blockchain;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main()
        {
            var builder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"],
                LocalStorageData = new LocalStorageData(AppDomain.CurrentDomain.BaseDirectory, 200)
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.Console,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));
            //builder.RegisterType<TutorMeSearch>().AsSelf();
            var container = builder.Build();
            var t = container.Resolve<IBlockchainProvider>();
            
            string senderAddress = "0x27e739f9dF8135fD1946b0b5584BcE49E22000af"; //Hadar
            string senderPK = "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"; // HadarPK
            string recipientAddress = "0xF8a5AD3Bc74196d759f7E021738AeC542a69Bc16"; //ubuntu address
            string azureUrl = "http://s256cw-dns-reg1.northeurope.cloudapp.azure.com:8545";
            var transactionHash = "0xa09db301ad49fb1e240f7fe6c4a70edadd9506d93278fb412b571cf8b2786aa4"; //ICO Contract Hash
            var abi = @"[{'anonymous': false,'inputs': [{'indexed': true,'name': 'from','type': 'address'},{'indexed': false,'name': 'value','type': 'uint256'}],'name': 'Burn','type': 'event'},{'constant': false,'inputs': [
                            {'name': '_spender','type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'approve',	'outputs': [{'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable',
                            'type': 'function'},{'constant': false,'inputs': [{'name': '_spender','type': 'address'},{'name': '_value','type': 'uint256'},{'name': '_extraData','type': 'bytes'}],'name': 'approveAndCall','outputs': [{'name': 'success',
                            'type': 'bool'}],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_value','type': 'uint256'}],'name': 'burn','outputs': [{'name': 'success','type': 'bool'}],
                            'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_from','type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'burnFrom','outputs': [
                            {'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [],'name': 'buy','outputs': [],'payable': true,'stateMutability': 'payable',
                            'type': 'function'},{'constant': false,	'inputs': [{'name': 'target','type': 'address'},{'name': 'freeze','type': 'bool'}],'name': 'freezeAccount','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'
                            },{'constant': false,'inputs': [{'name': 'target','type': 'address'},{'name': 'mintedAmount','type': 'uint256'}],'name': 'mintToken','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},
                            {'constant': false,'inputs': [{'name': 'amount','type': 'uint256'}],'name': 'sell','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'anonymous': false,'inputs': [{
                            'indexed': true,'name': 'from','type': 'address'},{'indexed': true,'name': 'to','type': 'address'},{'indexed': false,'name': 'value','type': 'uint256'}],'name': 'Transfer','type': 'event'},{'anonymous': false,
                            'inputs': [{'indexed': false,'name': 'target','type': 'address'},{'indexed': false,'name': 'frozen','type': 'bool'}],'name': 'FrozenFunds','type': 'event'},{'constant': false,'inputs': [{'name': 'newSellPrice',
                            'type': 'uint256'},{'name': 'newBuyPrice','type': 'uint256'}],'name': 'setPrices','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_to',
                            'type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'transfer','outputs': [],'payable': false,'stateMutability': 'nonpayable',	'type': 'function'},{'constant': false,'inputs': [{	'name': '_from',
                            'type': 'address'},{'name': '_to','type': 'address'},{'name': '_value',	'type': 'uint256'}],'name': 'transferFrom','outputs': [{'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable',
                            'type': 'function'},{'constant': false,'inputs': [{'name': 'newOwner','type': 'address'}],'name': 'transferOwnership','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{
                            'inputs': [{'name': 'initialSupply','type': 'uint256'},{'name': 'tokenName','type': 'string'},{'name': 'tokenSymbol','type': 'string'}],'payable': false,'stateMutability': 'nonpayable','type': 'constructor'
                            },{'constant': true,'inputs': [{'name': '','type': 'address'},{'name': '','type': 'address'}],'name': 'allowance','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view',
                            'type': 'function'},{'constant': true,'inputs': [{'name': '','type': 'address'}],'name': 'balanceOf','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'
                            },{'constant': true,'inputs': [],'name': 'buyPrice','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'decimals',
                            'outputs': [{'name': '','type': 'uint8'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [{	'name': '','type': 'address'}],'name': 'frozenAccount','outputs': [{
                            'name': '','type': 'bool'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'name','outputs': [{'name': '','type': 'string'}],'payable': false,'stateMutability': 'view',
                            'type': 'function'},{'constant': true,'inputs': [],'name': 'owner','outputs': [{'name': '',	'type': 'address'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'sellPrice',
                            'outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'symbol','outputs': [{'name': '','type': 'string'}],'payable': false,'stateMutability'
                            : 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'totalSupply','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'}]";
                            //ICO abi

            //var z = await t.SendTxAsync(senderAddress, senderPK, recipientAddress, azureUrl);
            //var Test = t.GetAddress(senderPK);
            var T = await t.MessageContractAsync<string>(transactionHash, azureUrl, abi, "symbol");
            var S = await t.TxContract(operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl)

            //var resolve2 = container
            //    .Resolve<IReadRepositoryAsync<(IEnumerable<CourseSearchWriteDto> update, IEnumerable<SearchWriteBaseDto>
            //        delete, long version), SyncAzureQuery>>();

            //var t2 = await resolve2.GetAsync(new SyncAzureQuery(0, 0), default);



            Console.WriteLine("Finish");
            Console.ReadLine();
        }





    }
}
