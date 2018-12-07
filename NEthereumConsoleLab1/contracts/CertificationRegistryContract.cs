using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace NEthereumConsoleLab1.contracts
{

    public class CertificationRegistryContract
    {
        public static string abi = "[{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"address\"}],\"name\":\"CertAdmins\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"GlobalAdmin\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"name\":\"CertificateAddresses\",\"outputs\":[{\"name\":\"Address\",\"type\":\"address\"},{\"name\":\"IsOrganization\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_certID\",\"type\":\"string\"},{\"indexed\":false,\"name\":\"_certAdrress\",\"type\":\"address\"}],\"name\":\"CertificationSet\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_certID\",\"type\":\"string\"},{\"indexed\":false,\"name\":\"_certAdrress\",\"type\":\"address\"}],\"name\":\"IndividualCertificationSet\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_certID\",\"type\":\"string\"},{\"indexed\":false,\"name\":\"_certAdrress\",\"type\":\"address\"}],\"name\":\"CertificationDeleted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_certAdmin\",\"type\":\"address\"}],\"name\":\"CertAdminAdded\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_certAdmin\",\"type\":\"address\"}],\"name\":\"CertAdminDeleted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"name\":\"_globalAdmin\",\"type\":\"address\"}],\"name\":\"GlobalAdminChanged\",\"type\":\"event\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CompanyName\",\"type\":\"string\"},{\"name\":\"_Norm\",\"type\":\"string\"},{\"name\":\"_CertID\",\"type\":\"string\"},{\"name\":\"_issued\",\"type\":\"uint256\"},{\"name\":\"_expires\",\"type\":\"uint256\"},{\"name\":\"_Scope\",\"type\":\"string\"},{\"name\":\"_issuingBody\",\"type\":\"string\"}],\"name\":\"setCertificate\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"},{\"name\":\"_hashValue\",\"type\":\"string\"}],\"name\":\"setIndividualCertificate\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"},{\"name\":\"_hashValue\",\"type\":\"string\"}],\"name\":\"updateIndividualCertificate\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"}],\"name\":\"delOrganizationCertificate\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"}],\"name\":\"delIndividualCertificate\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertAdmin\",\"type\":\"address\"}],\"name\":\"addCertAdmin\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_CertAdmin\",\"type\":\"address\"}],\"name\":\"delCertAdmin\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_GlobalAdmin\",\"type\":\"address\"}],\"name\":\"changeGlobalAdmin\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"}],\"name\":\"getCertAddressByID\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_CertID\",\"type\":\"string\"}],\"name\":\"getCertKey\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"}],\"payable\":false,\"stateMutability\":\"pure\",\"type\":\"function\"}]";

        public static string bytecode = "";

        private readonly string contractAddress;

        private readonly Web3 web3;

        private readonly ManagedAccount certAdminAccount;

        private readonly Contract contract;

        private readonly uint defaultGasLimit = 300000;
        
        public CertificationRegistryContract(
            Web3 _web3Conn, 
            ManagedAccount _certAdminAccount, 
            string _contractAddress)
        {
            web3 = _web3Conn;

            certAdminAccount = _certAdminAccount;

            contractAddress = _contractAddress;

            contract = web3.Eth.GetContract(abi, _contractAddress);

        }

        public Task<string> SetCertificate(
            string companyName,
            string norm,
            string certId,
            uint issuedDate,
            uint expireDate,
            string scope,
            string issuingBody,
            uint gasPrice
        )
        {
            var thisFunction = contract.GetFunction("setCertificate");
            var deployTask = thisFunction.SendTransactionAsync(
                certAdminAccount.Address,
                new HexBigInteger(defaultGasLimit),
                new HexBigInteger(gasPrice),
                null,
                new object[] {companyName, norm, certId, issuedDate, expireDate, scope, issuingBody});
            return deployTask;
        }

        public Task<string> SetIndividualCertificate(
            string certId,
            string firstBytes32, // should start with "0x"
            string lastBytes32, // should start with "0x"
            string organizationId, // should contain maximum 32 characters
            uint gasPrice)
        {
            if(!(firstBytes32.StartsWith("0x") && lastBytes32.StartsWith("0x")))
            {
                // TODO: Define more descriptive/domain-specific exception
                throw new Exception("Invalid function input");
            }

            if (organizationId.Length > 32 || 
                firstBytes32.Substring(2).Length > 32 || 
                lastBytes32.Substring(2).Length > 32)
            {
                throw new Exception("Invalid length of byte32 inputs");
            }

            var thisFunction = contract.GetFunction("setIndividualCertificate");
            var deployTask = thisFunction.SendTransactionAsync(
                certAdminAccount.Address,
                new HexBigInteger(defaultGasLimit),
                new HexBigInteger(gasPrice),
                null,
                new object[] {certId, firstBytes32, lastBytes32, organizationId});
            return deployTask;
        }

                
    }
}
