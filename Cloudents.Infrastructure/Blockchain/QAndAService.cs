﻿using Cloudents.Core.Interfaces;
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

namespace Cloudents.Infrastructure.Blockchain
{
    class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";

        protected override string TransactionHash => "0x430fdc71d7b86f432ae0d22d0cc11ce7909f0434942f5943f2288f3140dac07d";
        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task<string> SubmitQuestionAsync(long question, decimal price, string senderAddress,
            CancellationToken token)
        {
            // return null;
            var web3 = new Web3(_configurationKeys.BlockChainNetwork);
            // var unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);
            var parameters = (new object[] { new BigInteger(price), "0x55a885a9a1f7e8e5ca10a79ad7addcc5bc43f623", question});
            //var maxGas = new HexBigInteger(70000);
            string abi = await ReadAbiAsync(token);
           
            string byteCode = @"{
                            'linkReferences': { },
	                        'object': '606060405234156200001057600080fd5b6040516200122a3803806200122a833981016040528080519060200190919080519060200190919080518201919050508260008190555033600160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508060039080519060200190620000a09291906200023b565b5081600460006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663095ea7b3336200014b600054670de0b6b3a7640000620001ff6401000000000262000e6d176401000000009004565b6000604051602001526040518363ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050602060405180830381600087803b1515620001d957600080fd5b6102c65a03f11515620001eb57600080fd5b5050506040518051905050505050620002ea565b60008083141562000214576000905062000235565b81830290508183828115156200022657fe5b041415156200023157fe5b8090505b92915050565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106200027e57805160ff1916838001178555620002af565b82800160010185558215620002af579182015b82811115620002ae57825182559160200191906001019062000291565b5b509050620002be9190620002c2565b5090565b620002e791905b80821115620002e3576000816000905550600101620002c9565b5090565b90565b610f3080620002fa6000396000f3006060604052600436106100ba576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff168063015dd510146100bf57806318b308ff146100d457806336b8d704146100fa5780633fad9ae0146101235780635bcb2fc6146101b15780636e66f6e9146101c657806370a082311461021b578063a035b1fe14610268578063ba9703ce14610291578063beeb4c4c146102fb578063e24e494214610334578063ec859db514610389575b600080fd5b34156100ca57600080fd5b6100d26103ac565b005b34156100df57600080fd5b6100f8600480803560ff16906020019091905050610594565b005b341561010557600080fd5b61010d610698565b6040518082815260200191505060405180910390f35b341561012e57600080fd5b61013661069e565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561017657808201518184015260208101905061015b565b50505050905090810190601f1680156101a35780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34156101bc57600080fd5b6101c461073c565b005b34156101d157600080fd5b6101d961090d565b604051808273ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200191505060405180910390f35b341561022657600080fd5b610252600480803573ffffffffffffffffffffffffffffffffffffffff16906020019091905050610933565b6040518082815260200191505060405180910390f35b341561027357600080fd5b61027b61094b565b6040518082815260200191505060405180910390f35b341561029c57600080fd5b6102a4610951565b6040518080602001828103825283818151815260200191508051906020019060200280838360005b838110156102e75780820151818401526020810190506102cc565b505050509050019250505060405180910390f35b341561030657600080fd5b610332600480803573ffffffffffffffffffffffffffffffffffffffff16906020019091905050610a28565b005b341561033f57600080fd5b610347610bb5565b604051808273ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200191505060405180910390f35b341561039457600080fd5b6103aa6004808035906020019091905050610bdb565b005b600160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff1614151561040857600080fd5b600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663a9059cbb3361045b600054670de0b6b3a7640000610e6d565b6040518363ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050600060405180830381600087803b15156104df57600080fd5b6102c65a03f115156104f057600080fd5b50505061054f600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205461054a600054670de0b6b3a7640000610e6d565b610ea5565b600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550565b600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663a9059cbb336105fd6105ef6000546105ea8760ff166064610ebe565b610e6d565b670de0b6b3a7640000610e6d565b6040518363ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050600060405180830381600087803b151561068157600080fd5b6102c65a03f1151561069257600080fd5b50505050565b60025481565b60038054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156107345780601f1061070957610100808354040283529160200191610734565b820191906000526020600020905b81548152906001019060200180831161071757829003601f168201915b505050505081565b600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff166323b872dd3330610790600054670de0b6b3a7640000610e6d565b6000604051602001526040518463ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020018373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020018281526020019350505050602060405180830381600087803b151561085057600080fd5b6102c65a03f1151561086157600080fd5b50505060405180519050506108c8600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020546108c3600054670de0b6b3a7640000610e6d565b610ed4565b600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550565b600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b60056020528060005260406000206000915090505481565b60005481565b610959610ef0565b610961610ef0565b60006002546040518059106109735750595b90808252806020026020018201604052509150600090505b600254811015610a20576006600082815260200190815260200160002060009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1682828151811015156109d757fe5b9060200190602002019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff1681525050808060010191505061098b565b819250505090565b600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663a9059cbb82610a7b600054670de0b6b3a7640000610e6d565b6040518363ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050600060405180830381600087803b1515610aff57600080fd5b6102c65a03f11515610b1057600080fd5b505050610b6f600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054610b6a600054670de0b6b3a7640000610e6d565b610ea5565b600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190555050565b600160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b3360066000600254815260200190815260200160002060006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550610c3c6002546001610ed4565b600281905550600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1663095ea7b333610c9384670de0b6b3a7640000610e6d565b6000604051602001526040518363ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200182815260200192505050602060405180830381600087803b1515610d2057600080fd5b6102c65a03f11515610d3157600080fd5b5050506040518051905050600460009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff166323b872dd3330610d8e85670de0b6b3a7640000610e6d565b6000604051602001526040518463ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020018373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020018281526020019350505050602060405180830381600087803b1515610e4e57600080fd5b6102c65a03f11515610e5f57600080fd5b505050604051805190505050565b600080831415610e805760009050610e9f565b8183029050818382811515610e9157fe5b04141515610e9b57fe5b8090505b92915050565b6000828211151515610eb357fe5b818303905092915050565b60008183811515610ecb57fe5b04905092915050565b60008183019050828110151515610ee757fe5b80905092915050565b6020604051908101604052806000815250905600a165627a7a723058202787e98178689f162f6a91e9137af0c422c135da96f3b1af5922b4c33c888f8a0029',
	                        'opcodes': 'PUSH1 0x60 PUSH1 0x40 MSTORE CALLVALUE ISZERO PUSH3 0x10 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH1 0x40 MLOAD PUSH3 0x122A CODESIZE SUB DUP1 PUSH3 0x122A DUP4 CODECOPY DUP2 ADD PUSH1 0x40 MSTORE DUP1 DUP1 MLOAD SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 DUP1 MLOAD SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 DUP1 MLOAD DUP3 ADD SWAP2 SWAP1 POP POP DUP3 PUSH1 0x0 DUP2 SWAP1 SSTORE POP CALLER PUSH1 0x1 PUSH1 0x0 PUSH2 0x100 EXP DUP2 SLOAD DUP2 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF MUL NOT AND SWAP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND MUL OR SWAP1 SSTORE POP DUP1 PUSH1 0x3 SWAP1 DUP1 MLOAD SWAP1 PUSH1 0x20 ADD SWAP1 PUSH3 0xA0 SWAP3 SWAP2 SWAP1 PUSH3 0x23B JUMP JUMPDEST POP DUP2 PUSH1 0x4 PUSH1 0x0 PUSH2 0x100 EXP DUP2 SLOAD DUP2 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF MUL NOT AND SWAP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND MUL OR SWAP1 SSTORE POP PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0x95EA7B3 CALLER PUSH3 0x14B PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH3 0x1FF PUSH5 0x100000000 MUL PUSH3 0xE6D OR PUSH5 0x100000000 SWAP1 DIV JUMP JUMPDEST PUSH1 0x0 PUSH1 0x40 MLOAD PUSH1 0x20 ADD MSTORE PUSH1 0x40 MLOAD DUP4 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP3 POP POP POP PUSH1 0x20 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH3 0x1D9 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH3 0x1EB JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH1 0x40 MLOAD DUP1 MLOAD SWAP1 POP POP POP POP POP PUSH3 0x2EA JUMP JUMPDEST PUSH1 0x0 DUP1 DUP4 EQ ISZERO PUSH3 0x214 JUMPI PUSH1 0x0 SWAP1 POP PUSH3 0x235 JUMP JUMPDEST DUP2 DUP4 MUL SWAP1 POP DUP2 DUP4 DUP3 DUP2 ISZERO ISZERO PUSH3 0x226 JUMPI INVALID JUMPDEST DIV EQ ISZERO ISZERO PUSH3 0x231 JUMPI INVALID JUMPDEST DUP1 SWAP1 POP JUMPDEST SWAP3 SWAP2 POP POP JUMP JUMPDEST DUP3 DUP1 SLOAD PUSH1 0x1 DUP2 PUSH1 0x1 AND ISZERO PUSH2 0x100 MUL SUB AND PUSH1 0x2 SWAP1 DIV SWAP1 PUSH1 0x0 MSTORE PUSH1 0x20 PUSH1 0x0 KECCAK256 SWAP1 PUSH1 0x1F ADD PUSH1 0x20 SWAP1 DIV DUP2 ADD SWAP3 DUP3 PUSH1 0x1F LT PUSH3 0x27E JUMPI DUP1 MLOAD PUSH1 0xFF NOT AND DUP4 DUP1 ADD OR DUP6 SSTORE PUSH3 0x2AF JUMP JUMPDEST DUP3 DUP1 ADD PUSH1 0x1 ADD DUP6 SSTORE DUP3 ISZERO PUSH3 0x2AF JUMPI SWAP2 DUP3 ADD JUMPDEST DUP3 DUP2 GT ISZERO PUSH3 0x2AE JUMPI DUP3 MLOAD DUP3 SSTORE SWAP2 PUSH1 0x20 ADD SWAP2 SWAP1 PUSH1 0x1 ADD SWAP1 PUSH3 0x291 JUMP JUMPDEST JUMPDEST POP SWAP1 POP PUSH3 0x2BE SWAP2 SWAP1 PUSH3 0x2C2 JUMP JUMPDEST POP SWAP1 JUMP JUMPDEST PUSH3 0x2E7 SWAP2 SWAP1 JUMPDEST DUP1 DUP3 GT ISZERO PUSH3 0x2E3 JUMPI PUSH1 0x0 DUP2 PUSH1 0x0 SWAP1 SSTORE POP PUSH1 0x1 ADD PUSH3 0x2C9 JUMP JUMPDEST POP SWAP1 JUMP JUMPDEST SWAP1 JUMP JUMPDEST PUSH2 0xF30 DUP1 PUSH3 0x2FA PUSH1 0x0 CODECOPY PUSH1 0x0 RETURN STOP PUSH1 0x60 PUSH1 0x40 MSTORE PUSH1 0x4 CALLDATASIZE LT PUSH2 0xBA JUMPI PUSH1 0x0 CALLDATALOAD PUSH29 0x100000000000000000000000000000000000000000000000000000000 SWAP1 DIV PUSH4 0xFFFFFFFF AND DUP1 PUSH4 0x15DD510 EQ PUSH2 0xBF JUMPI DUP1 PUSH4 0x18B308FF EQ PUSH2 0xD4 JUMPI DUP1 PUSH4 0x36B8D704 EQ PUSH2 0xFA JUMPI DUP1 PUSH4 0x3FAD9AE0 EQ PUSH2 0x123 JUMPI DUP1 PUSH4 0x5BCB2FC6 EQ PUSH2 0x1B1 JUMPI DUP1 PUSH4 0x6E66F6E9 EQ PUSH2 0x1C6 JUMPI DUP1 PUSH4 0x70A08231 EQ PUSH2 0x21B JUMPI DUP1 PUSH4 0xA035B1FE EQ PUSH2 0x268 JUMPI DUP1 PUSH4 0xBA9703CE EQ PUSH2 0x291 JUMPI DUP1 PUSH4 0xBEEB4C4C EQ PUSH2 0x2FB JUMPI DUP1 PUSH4 0xE24E4942 EQ PUSH2 0x334 JUMPI DUP1 PUSH4 0xEC859DB5 EQ PUSH2 0x389 JUMPI JUMPDEST PUSH1 0x0 DUP1 REVERT JUMPDEST CALLVALUE ISZERO PUSH2 0xCA JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0xD2 PUSH2 0x3AC JUMP JUMPDEST STOP JUMPDEST CALLVALUE ISZERO PUSH2 0xDF JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0xF8 PUSH1 0x4 DUP1 DUP1 CALLDATALOAD PUSH1 0xFF AND SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 POP POP PUSH2 0x594 JUMP JUMPDEST STOP JUMPDEST CALLVALUE ISZERO PUSH2 0x105 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x10D PUSH2 0x698 JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x12E JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x136 PUSH2 0x69E JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP1 PUSH1 0x20 ADD DUP3 DUP2 SUB DUP3 MSTORE DUP4 DUP2 DUP2 MLOAD DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP DUP1 MLOAD SWAP1 PUSH1 0x20 ADD SWAP1 DUP1 DUP4 DUP4 PUSH1 0x0 JUMPDEST DUP4 DUP2 LT ISZERO PUSH2 0x176 JUMPI DUP1 DUP3 ADD MLOAD DUP2 DUP5 ADD MSTORE PUSH1 0x20 DUP2 ADD SWAP1 POP PUSH2 0x15B JUMP JUMPDEST POP POP POP POP SWAP1 POP SWAP1 DUP2 ADD SWAP1 PUSH1 0x1F AND DUP1 ISZERO PUSH2 0x1A3 JUMPI DUP1 DUP3 SUB DUP1 MLOAD PUSH1 0x1 DUP4 PUSH1 0x20 SUB PUSH2 0x100 EXP SUB NOT AND DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP JUMPDEST POP SWAP3 POP POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x1BC JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x1C4 PUSH2 0x73C JUMP JUMPDEST STOP JUMPDEST CALLVALUE ISZERO PUSH2 0x1D1 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x1D9 PUSH2 0x90D JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP3 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x226 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x252 PUSH1 0x4 DUP1 DUP1 CALLDATALOAD PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 POP POP PUSH2 0x933 JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x273 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x27B PUSH2 0x94B JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x29C JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2A4 PUSH2 0x951 JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP1 PUSH1 0x20 ADD DUP3 DUP2 SUB DUP3 MSTORE DUP4 DUP2 DUP2 MLOAD DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP DUP1 MLOAD SWAP1 PUSH1 0x20 ADD SWAP1 PUSH1 0x20 MUL DUP1 DUP4 DUP4 PUSH1 0x0 JUMPDEST DUP4 DUP2 LT ISZERO PUSH2 0x2E7 JUMPI DUP1 DUP3 ADD MLOAD DUP2 DUP5 ADD MSTORE PUSH1 0x20 DUP2 ADD SWAP1 POP PUSH2 0x2CC JUMP JUMPDEST POP POP POP POP SWAP1 POP ADD SWAP3 POP POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x306 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x332 PUSH1 0x4 DUP1 DUP1 CALLDATALOAD PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 POP POP PUSH2 0xA28 JUMP JUMPDEST STOP JUMPDEST CALLVALUE ISZERO PUSH2 0x33F JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x347 PUSH2 0xBB5 JUMP JUMPDEST PUSH1 0x40 MLOAD DUP1 DUP3 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP2 POP POP PUSH1 0x40 MLOAD DUP1 SWAP2 SUB SWAP1 RETURN JUMPDEST CALLVALUE ISZERO PUSH2 0x394 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x3AA PUSH1 0x4 DUP1 DUP1 CALLDATALOAD SWAP1 PUSH1 0x20 ADD SWAP1 SWAP2 SWAP1 POP POP PUSH2 0xBDB JUMP JUMPDEST STOP JUMPDEST PUSH1 0x1 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND EQ ISZERO ISZERO PUSH2 0x408 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0xA9059CBB CALLER PUSH2 0x45B PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x40 MLOAD DUP4 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP3 POP POP POP PUSH1 0x0 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0x4DF JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0x4F0 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH2 0x54F PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 SLOAD PUSH2 0x54A PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH2 0xEA5 JUMP JUMPDEST PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 DUP2 SWAP1 SSTORE POP JUMP JUMPDEST PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0xA9059CBB CALLER PUSH2 0x5FD PUSH2 0x5EF PUSH1 0x0 SLOAD PUSH2 0x5EA DUP8 PUSH1 0xFF AND PUSH1 0x64 PUSH2 0xEBE JUMP JUMPDEST PUSH2 0xE6D JUMP JUMPDEST PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x40 MLOAD DUP4 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP3 POP POP POP PUSH1 0x0 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0x681 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0x692 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP POP JUMP JUMPDEST PUSH1 0x2 SLOAD DUP2 JUMP JUMPDEST PUSH1 0x3 DUP1 SLOAD PUSH1 0x1 DUP2 PUSH1 0x1 AND ISZERO PUSH2 0x100 MUL SUB AND PUSH1 0x2 SWAP1 DIV DUP1 PUSH1 0x1F ADD PUSH1 0x20 DUP1 SWAP2 DIV MUL PUSH1 0x20 ADD PUSH1 0x40 MLOAD SWAP1 DUP2 ADD PUSH1 0x40 MSTORE DUP1 SWAP3 SWAP2 SWAP1 DUP2 DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP1 SLOAD PUSH1 0x1 DUP2 PUSH1 0x1 AND ISZERO PUSH2 0x100 MUL SUB AND PUSH1 0x2 SWAP1 DIV DUP1 ISZERO PUSH2 0x734 JUMPI DUP1 PUSH1 0x1F LT PUSH2 0x709 JUMPI PUSH2 0x100 DUP1 DUP4 SLOAD DIV MUL DUP4 MSTORE SWAP2 PUSH1 0x20 ADD SWAP2 PUSH2 0x734 JUMP JUMPDEST DUP3 ADD SWAP2 SWAP1 PUSH1 0x0 MSTORE PUSH1 0x20 PUSH1 0x0 KECCAK256 SWAP1 JUMPDEST DUP2 SLOAD DUP2 MSTORE SWAP1 PUSH1 0x1 ADD SWAP1 PUSH1 0x20 ADD DUP1 DUP4 GT PUSH2 0x717 JUMPI DUP3 SWAP1 SUB PUSH1 0x1F AND DUP3 ADD SWAP2 JUMPDEST POP POP POP POP POP DUP2 JUMP JUMPDEST PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0x23B872DD CALLER ADDRESS PUSH2 0x790 PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x0 PUSH1 0x40 MLOAD PUSH1 0x20 ADD MSTORE PUSH1 0x40 MLOAD DUP5 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP5 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP4 POP POP POP POP PUSH1 0x20 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0x850 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0x861 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH1 0x40 MLOAD DUP1 MLOAD SWAP1 POP POP PUSH2 0x8C8 PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 SLOAD PUSH2 0x8C3 PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH2 0xED4 JUMP JUMPDEST PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 DUP2 SWAP1 SSTORE POP JUMP JUMPDEST PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 JUMP JUMPDEST PUSH1 0x5 PUSH1 0x20 MSTORE DUP1 PUSH1 0x0 MSTORE PUSH1 0x40 PUSH1 0x0 KECCAK256 PUSH1 0x0 SWAP2 POP SWAP1 POP SLOAD DUP2 JUMP JUMPDEST PUSH1 0x0 SLOAD DUP2 JUMP JUMPDEST PUSH2 0x959 PUSH2 0xEF0 JUMP JUMPDEST PUSH2 0x961 PUSH2 0xEF0 JUMP JUMPDEST PUSH1 0x0 PUSH1 0x2 SLOAD PUSH1 0x40 MLOAD DUP1 MSIZE LT PUSH2 0x973 JUMPI POP MSIZE JUMPDEST SWAP1 DUP1 DUP3 MSTORE DUP1 PUSH1 0x20 MUL PUSH1 0x20 ADD DUP3 ADD PUSH1 0x40 MSTORE POP SWAP2 POP PUSH1 0x0 SWAP1 POP JUMPDEST PUSH1 0x2 SLOAD DUP2 LT ISZERO PUSH2 0xA20 JUMPI PUSH1 0x6 PUSH1 0x0 DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP3 DUP3 DUP2 MLOAD DUP2 LT ISZERO ISZERO PUSH2 0x9D7 JUMPI INVALID JUMPDEST SWAP1 PUSH1 0x20 ADD SWAP1 PUSH1 0x20 MUL ADD SWAP1 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND SWAP1 DUP2 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE POP POP DUP1 DUP1 PUSH1 0x1 ADD SWAP2 POP POP PUSH2 0x98B JUMP JUMPDEST DUP2 SWAP3 POP POP POP SWAP1 JUMP JUMPDEST PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0xA9059CBB DUP3 PUSH2 0xA7B PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x40 MLOAD DUP4 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP3 POP POP POP PUSH1 0x0 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0xAFF JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0xB10 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH2 0xB6F PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 SLOAD PUSH2 0xB6A PUSH1 0x0 SLOAD PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH2 0xEA5 JUMP JUMPDEST PUSH1 0x5 PUSH1 0x0 CALLER PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 DUP2 SWAP1 SSTORE POP POP JUMP JUMPDEST PUSH1 0x1 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 JUMP JUMPDEST CALLER PUSH1 0x6 PUSH1 0x0 PUSH1 0x2 SLOAD DUP2 MSTORE PUSH1 0x20 ADD SWAP1 DUP2 MSTORE PUSH1 0x20 ADD PUSH1 0x0 KECCAK256 PUSH1 0x0 PUSH2 0x100 EXP DUP2 SLOAD DUP2 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF MUL NOT AND SWAP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND MUL OR SWAP1 SSTORE POP PUSH2 0xC3C PUSH1 0x2 SLOAD PUSH1 0x1 PUSH2 0xED4 JUMP JUMPDEST PUSH1 0x2 DUP2 SWAP1 SSTORE POP PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0x95EA7B3 CALLER PUSH2 0xC93 DUP5 PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x0 PUSH1 0x40 MLOAD PUSH1 0x20 ADD MSTORE PUSH1 0x40 MLOAD DUP4 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP3 POP POP POP PUSH1 0x20 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0xD20 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0xD31 JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH1 0x40 MLOAD DUP1 MLOAD SWAP1 POP POP PUSH1 0x4 PUSH1 0x0 SWAP1 SLOAD SWAP1 PUSH2 0x100 EXP SWAP1 DIV PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH4 0x23B872DD CALLER ADDRESS PUSH2 0xD8E DUP6 PUSH8 0xDE0B6B3A7640000 PUSH2 0xE6D JUMP JUMPDEST PUSH1 0x0 PUSH1 0x40 MLOAD PUSH1 0x20 ADD MSTORE PUSH1 0x40 MLOAD DUP5 PUSH4 0xFFFFFFFF AND PUSH29 0x100000000000000000000000000000000000000000000000000000000 MUL DUP2 MSTORE PUSH1 0x4 ADD DUP1 DUP5 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP4 PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND PUSH20 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF AND DUP2 MSTORE PUSH1 0x20 ADD DUP3 DUP2 MSTORE PUSH1 0x20 ADD SWAP4 POP POP POP POP PUSH1 0x20 PUSH1 0x40 MLOAD DUP1 DUP4 SUB DUP2 PUSH1 0x0 DUP8 DUP1 EXTCODESIZE ISZERO ISZERO PUSH2 0xE4E JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST PUSH2 0x2C6 GAS SUB CALL ISZERO ISZERO PUSH2 0xE5F JUMPI PUSH1 0x0 DUP1 REVERT JUMPDEST POP POP POP PUSH1 0x40 MLOAD DUP1 MLOAD SWAP1 POP POP POP JUMP JUMPDEST PUSH1 0x0 DUP1 DUP4 EQ ISZERO PUSH2 0xE80 JUMPI PUSH1 0x0 SWAP1 POP PUSH2 0xE9F JUMP JUMPDEST DUP2 DUP4 MUL SWAP1 POP DUP2 DUP4 DUP3 DUP2 ISZERO ISZERO PUSH2 0xE91 JUMPI INVALID JUMPDEST DIV EQ ISZERO ISZERO PUSH2 0xE9B JUMPI INVALID JUMPDEST DUP1 SWAP1 POP JUMPDEST SWAP3 SWAP2 POP POP JUMP JUMPDEST PUSH1 0x0 DUP3 DUP3 GT ISZERO ISZERO ISZERO PUSH2 0xEB3 JUMPI INVALID JUMPDEST DUP2 DUP4 SUB SWAP1 POP SWAP3 SWAP2 POP POP JUMP JUMPDEST PUSH1 0x0 DUP2 DUP4 DUP2 ISZERO ISZERO PUSH2 0xECB JUMPI INVALID JUMPDEST DIV SWAP1 POP SWAP3 SWAP2 POP POP JUMP JUMPDEST PUSH1 0x0 DUP2 DUP4 ADD SWAP1 POP DUP3 DUP2 LT ISZERO ISZERO ISZERO PUSH2 0xEE7 JUMPI INVALID JUMPDEST DUP1 SWAP1 POP SWAP3 SWAP2 POP POP JUMP JUMPDEST PUSH1 0x20 PUSH1 0x40 MLOAD SWAP1 DUP2 ADD PUSH1 0x40 MSTORE DUP1 PUSH1 0x0 DUP2 MSTORE POP SWAP1 JUMP STOP LOG1 PUSH6 0x627A7A723058 KECCAK256 0x27 DUP8 0xe9 DUP2 PUSH25 0x689F162F6A91E9137AF0C422C135DA96F3B1AF5922B4C33C88 DUP16 DUP11 STOP 0x29 ',
	                        'sourceMap': '89:2332:1:-;;;551:326;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;664:6;656:5;:14;;;;690:10;681:6;;:19;;;;;;;;;;;;;;;;;;722:9;711:8;:20;;;;;;;;;;;;:::i;:::-;;770:27;742:11;;:56;;;;;;;;;;;;;;;;;;809:11;;;;;;;;;;;:19;;;829:10;841:27;854:5;;861:6;841:12;;;;;:27;;;:::i;:::-;809:60;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;551:326;;;89:2332;;202:167:2;260:9;286:1;281;:6;277:35;;;304:1;297:8;;;;277:35;325:1;321;:5;317:9;;348:1;343;339;:5;;;;;;;;:10;332:18;;;;;;363:1;356:8;;202:167;;;;;:::o;89:2332:1:-;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;:::i;:::-;;;:::o;:::-;;;;;;;;;;;;;;;;;;;;;;;;;;;:::o;:::-;;;;;;;'
                        }";
            var unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, "Cloudents69", 120, 10101010);
            var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, senderAddress,  parameters);
            return transactionHash;
        }
    }
}
