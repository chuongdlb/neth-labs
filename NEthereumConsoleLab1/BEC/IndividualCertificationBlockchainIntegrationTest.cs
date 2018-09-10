using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.ABI.Util;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3.Accounts.Managed;
using Xunit;
using Xunit.Abstractions;

namespace NEthereumConsoleLab1.BEC
{
    public class IndividualCertificationBlockchainIntegrationTest
    {
        private const string WEB3_HOST = "http://localhost:8545/";
        private const string PASSWORD = "password";
        private const string SENDER = "0x12890d2cce102216644c59dae5baed380d84830c";
        private readonly ITestOutputHelper output;

        public IndividualCertificationBlockchainIntegrationTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        private string hashValue =
            "f6ae0a78e422de5e72b4b848d934f74db036c68ecee8083401b2d334b1997c21f614ee8d3f040f8f1e795ddb61ca9557adfeb9001bc2d9f0782abfdcdb21e0af";

        private List<string> hashList = new List<string>(new string[]
        {
            "f6ae0a78e422de5e72b4b848d934f74db036c68ecee8083401b2d334b1997c21f614ee8d3f040f8f1e795ddb61ca9557adfeb9001bc2d9f0782abfdcdb21e000",
            "f6ae0a78e422de5e72b4b848d934f74db036c68ecee8083401b2d334b1997c21f614ee8d3f040f8f1e795ddb61ca9557adfeb9001bc2d9f0782abfdcdb21e001",
            "f6ae0a78e422de5e72b4b848d934f74db036c68ecee8083401b2d334b1997c21f614ee8d3f040f8f1e795ddb61ca9557adfeb9001bc2d9f0782abfdcdb21e002",
            "f6ae0a78e422de5e72b4b848d934f74db036c68ecee8083401b2d334b1997c21f614ee8d3f040f8f1e795ddb61ca9557adfeb9001bc2d9f0782abfdcdb21e004"
        });

        private const string byteCode =
            "0x608060405234801561001057600080fd5b50604051610679380380610679833981018060405281019080805182019291905050508060608190506080815114151561004957600080fd5b336000806101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550826001908051906020019061009f9291906100a8565b5050505061014d565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106100e957805160ff1916838001178555610117565b82800160010185558215610117579182015b828111156101165782518255916020019190600101906100fb565b5b5090506101249190610128565b5090565b61014a91905b8082111561014657600081600090555060010161012e565b5090565b90565b61051d8061015c6000396000f300608060405260043610610062576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680632b92b8e51461006757806349cf2eae146100f757806361cbe2151461014e578063afa936b8146101b7575b600080fd5b34801561007357600080fd5b5061007c6101ce565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100bc5780820151818401526020810190506100a1565b50505050905090810190601f1680156100e95780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34801561010357600080fd5b5061010c61026c565b604051808273ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200191505060405180910390f35b34801561015a57600080fd5b506101b5600480360381019080803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509192919290505050610291565b005b3480156101c357600080fd5b506101cc6103d8565b005b60018054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156102645780601f1061023957610100808354040283529160200191610264565b820191906000526020600020905b81548152906001019060200180831161024757829003601f168201915b505050505081565b6000809054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b806060819050608081511415156102a757600080fd5b6000809054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff1614151561030257600080fd5b7f767a54fbccad24d3e60df26c9381cb44ebf6b2c9bc061a18b5ddfccafcd961a9600160405180806020018281038252838181546001816001161561010002031660029004815260200191508054600181600116156101000203166002900480156103ae5780601f10610383576101008083540402835291602001916103ae565b820191906000526020600020905b81548152906001019060200180831161039157829003601f168201915b50509250505060405180910390a182600190805190602001906103d292919061044c565b50505050565b6000809054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff1614151561043357600080fd5b3273ffffffffffffffffffffffffffffffffffffffff16ff5b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f1061048d57805160ff19168380011785556104bb565b828001600101855582156104bb579182015b828111156104ba57825182559160200191906001019061049f565b5b5090506104c891906104cc565b5090565b6104ee91905b808211156104ea5760008160009055506001016104d2565b5090565b905600a165627a7a7230582048102a130cb5668228f7411cc3e6681df82e015df12a4e3e2be6d30f745182340029";

        private const string contractAbi =
            @"[{""constant"":true,""inputs"":[],""name"":""hashValue"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""certifierAddress"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""_hashValue"",""type"":""string""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""first32bytesOfHash"",""type"":""bytes32""}],""name"":""HashValueSet"",""type"":""event""},{""constant"":false,""inputs"":[{""name"":""_hashValue"",""type"":""string""}],""name"":""setHashValue"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";


        [Fact]
        public async Task ShouldDeployContract()
        {

            var account = new ManagedAccount(SENDER, PASSWORD);
            var web3 = new Web3Geth(account);
            var gas = new HexBigInteger(3000000);

            var txId = await web3.Eth.DeployContract
                .SendRequestAsync(contractAbi, byteCode, SENDER, gas, hashValue);
            TransactionReceipt receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            var timer = new Stopwatch();
            timer.Start();
            // Wait receipt forever
            while(receipt == null) {
                Thread.Sleep(1000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            }
            timer.Stop();
            var elapsedTime = timer.ElapsedMilliseconds;
            output.WriteLine("Deployment time: {0} (s)", TimeSpan.FromMilliseconds(elapsedTime).Seconds);
            var contractAddress = receipt.ContractAddress;
            var contract = web3.Eth.GetContract(contractAbi, contractAddress);
            var hashValueGetter = contract.GetFunction("hashValue");
            var result = await hashValueGetter.CallAsync<string>();
            
            Assert.Equal(hashValue, result);

        }

        [Fact]
        public async Task ShouldBulkDeployContract()
        {
            var gas = new HexBigInteger(3000000);
            var account = new ManagedAccount(SENDER, PASSWORD);
            var web3 = new Web3Geth(account);

            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var timer = new Stopwatch();
            timer.Start();
            IEnumerable<Task<TransactionReceipt>> txReceiptTaskQuery = hashList.Select(hash =>
                web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                    contractAbi,
                    byteCode,
                    SENDER,
                    gas,
                    cancellationToken,
                    hash
                ));
            TransactionReceipt[] receipts = await Task.WhenAll(txReceiptTaskQuery);
            timer.Stop();
            output.WriteLine("Deployment elapsed time: {0} (s)", TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds).Seconds);

            foreach (var transactionReceipt in receipts)
            {
                var contract = web3.Eth.GetContract(contractAbi, transactionReceipt.ContractAddress);
                var hashFunc = contract.GetFunction("hashValue");
                var reHashValue = await hashFunc.CallAsync<string>();
                Assert.Contains(reHashValue, hashList);
            }

        }

        [Fact]
        public async Task ShouldNotDeployContractDueToInsufficientGas()
        {
            var senderAddress = "0x12890d2cce102216644c59dae5baed380d84830c";
            var gas = new HexBigInteger(3000);
            var account = new ManagedAccount(SENDER, PASSWORD);
            var web3 = new Web3Geth(account);
            
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            await Assert.ThrowsAsync<RpcResponseException>(() =>
            web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                contractAbi,
                byteCode,
                senderAddress,
                gas,
                cancellationToken,
                hashValue
                ));
        }

        [Fact]
        public async Task ShouldCancelDeploymentDueToShortTimeout()
        {
            var gas = new HexBigInteger(3000000);
            var account = new ManagedAccount(SENDER, PASSWORD);
            var web3 = new Web3Geth(account);

            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
            await Assert.ThrowsAsync<OperationCanceledException> (() =>
                web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                contractAbi,
                byteCode,
                SENDER,
                gas,
                cancellationToken,
                hashValue
            ));
        }


        [Fact]
        public async Task ShouldDeployWithFixedGasPrice()
        {
            var gas = new HexBigInteger(3000000);
            var account = new ManagedAccount(SENDER, PASSWORD);
            var web3 = new Web3Geth(account);
            
            var estimatedGasConsumed = await web3.Eth.DeployContract.EstimateGasAsync(contractAbi, byteCode, SENDER, hashValue);
            var avgGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            var unitConversion = new UnitConversion();
            var gasPrice = unitConversion.FromWei(avgGasPrice);
            output.WriteLine("Estimate gas: {0} & Average Gas price: {1}", estimatedGasConsumed.Value.ToString(), gasPrice.ToString());
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            TransactionReceipt txReceipt = await
                web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                    contractAbi,
                    byteCode,
                    SENDER,
                    gas,
                    avgGasPrice,
                    null,
                    cancellationToken,
                    hashValue
                );
            var contract = web3.Eth.GetContract(contractAbi, txReceipt.ContractAddress);
            var hashFunc = contract.GetFunction("hashValue");
            var reHashValue = await hashFunc.CallAsync<string>();
            Assert.Equal(hashValue, reHashValue);

        }
    }
}
