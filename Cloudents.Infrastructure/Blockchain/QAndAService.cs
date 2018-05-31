using Cloudents.Core.Interfaces;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;
using Cloudents.Infrastructure.BlockChain;
using Cloudents.Core.Models;
using System.Collections.Generic;


namespace Cloudents.Infrastructure.Blockchain
{
    class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";

        protected override string TransactionHash => "0x20327f5f3836cfdcbc5b38d49eac517cbf532134973c15a653cac2eb68b65dfd";

        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task SubmitQuestionAsync(string senderPk, long question, decimal price,
            CancellationToken token)
        {
           
            var web3 = new Web3(_configurationKeys.BlockChainNetwork);

            var contract = await GetContractAsync(GenerateWeb3Instance(senderPk), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("submitNewQuestion");
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { question, price };
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPk), maxGas, null, null, parameters).ConfigureAwait(false);
           
        }

        public async Task SubmitAnswerAsync(string address, long question, Guid answerId, CancellationToken token)
        {
            var web3 = new Web3(_configurationKeys.BlockChainNetwork);
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("submitNewAnswer");
            var maxGas = new HexBigInteger(3000000);
           
            var parameters = new object[] { question, answerId.ToString() };
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, maxGas, null, null, parameters).ConfigureAwait(false);
            
        }

        public async Task MarkAsCorrectAsync(string address, long question, CancellationToken token)
        {
            var web3 = new Web3(_configurationKeys.BlockChainNetwork);
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("approveAnswer");
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { question, address };
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, maxGas, null, null, parameters).ConfigureAwait(false);
        }

        public async Task UpVoteAsync(string address, long question, Guid answerId, decimal price, CancellationToken token)
        {
            var web3 = new Web3(_configurationKeys.BlockChainNetwork);
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("upVote");
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { question, answerId.ToString(), price };
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, maxGas, null, null, parameters).ConfigureAwait(false);
        }

        public async Task<string[]> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(), token).ConfigureAwait(false);
            var function = contract.GetFunction("returnUpVoteList");
            var parameters = new object[] { questionId, answerId.ToString() };
            List<string> tempResult = new List<string>();
            tempResult = await function.CallAsync<List<string>>(questionId, answerId.ToString()).ConfigureAwait(false);

            string[] result = new string[tempResult.Count];
            for (int i=0; i < tempResult.Count; i++)
            {
                result[i] = tempResult[i].ToString();
            }
            return result;
        }

        //public async Task<string> UpVoteReword(long questionId, Guid answer, string upVoterAddr, CancellationToken token)
        //{

        //}
    }
}
