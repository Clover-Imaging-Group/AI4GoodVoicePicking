using System;
using System.Linq;
using System.Threading.Tasks;
using ConversationalAI.API.Helpers;
using ConversationalAI.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConversationalAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScannerController : Controller
    {
        

        private readonly ILogger<ScannerController> _logger;
        private readonly string _smpQuery = $@"
                {{
                  attributes {{
                    id
                    onEquipment {{
                        displayName
                    }}
                    intValue
                    stateValue
                    description
                    }}
                }}";
        
        public ScannerController(ILogger<ScannerController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<WarehouseEquipmentList> Get()
        {
            
            try
            {
                var smpResponse= await GraphQLHelper.PerformGraphQLRequest(_smpQuery);
                return JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        [HttpGet]
        [Route("/GetAvailable")]
        public async Task<WarehouseEquipmentList> GetAvailableItems()
        {
            
            try
            {
                var smpResponse= await GraphQLHelper.PerformGraphQLRequest(_smpQuery);
                var queriedEquipment =  JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);

                if (queriedEquipment != null)
                    return new WarehouseEquipmentList
                    {
                        WarehouseEquipment = queriedEquipment.WarehouseEquipment.Where(item => item.Quantity > 0).ToList()
                    };

                return new WarehouseEquipmentList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("/restock")]
        public async Task Restock()
        {
            var smpResponse = await GraphQLHelper.PerformGraphQLRequest(_smpQuery);
            var equipmentList = JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);

            if (equipmentList?.WarehouseEquipment != null)
                foreach (var equipment in equipmentList.WarehouseEquipment)
                {
                    var mutationQuery = $@"
                             mutation updateAttributeMutation {{
                              updateAttribute(input: {{ id: ""{equipment.Id.ToString()}"", patch: {{intValue: ""3""}}}})  {{
                                    clientMutationId
                                    attribute {{
                                        id
                                        intValue
                                    }}
                                }}
                            }}";
                    await GraphQLHelper.PerformGraphQLRequest(mutationQuery);
                }
        }


        [HttpGet]
        [Route("/PickItem")]
        public async Task<string> PickItem(string equipment)
        {
            var smpResponse = await GraphQLHelper.PerformGraphQLRequest(_smpQuery);
            var equipmentList = JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);
            
            var equipmentToPick = equipmentList?.WarehouseEquipment.Find(x => x.DisplayName == equipment);
            
            if (equipmentToPick == null) 
                return $@"Sorry, {equipment} was not found";
            

            var updatedQuantity = equipmentToPick.Quantity is > 0 ? equipmentToPick.Quantity.Value - 1 : 0;
            
            var mutationQuery = $@"
                 mutation updateAttributeMutation {{
                  updateAttribute(input: {{ id: ""{equipmentToPick.Id.ToString()}"", patch: {{intValue: ""{updatedQuantity.ToString()}""}}}})  {{
                        clientMutationId
                        attribute {{
                            id
                            intValue
                        }}
                    }}
                }}";

            // return mutationQuery;
            var mutResponse= await GraphQLHelper.PerformGraphQLRequest(mutationQuery);
            Console.WriteLine(mutResponse);
            // return JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);
            if(updatedQuantity == 0)
                return $@"{equipment} was picked and is now out of stock";
            
            return $@"{equipment} was picked";
        }

        [HttpPost]
        public async Task<string> Post([FromBody] EquipmentQuantityUpdateRequest updateRequest)
        {
            var smpQuery = $@"
                mutation {{
                  updateAttribute(input: {{patch: {{intValue: ""54""}}, id: ""2053"" }}) 
                    {{
            clientMutationId
                 attribute {{
                     id
                     intValue
                 }}
                }}
                }}";
            try
            {
                var smpResponse= await GraphQLHelper.PerformGraphQLMutation(smpQuery);
                Console.WriteLine(smpResponse);
                // return JsonConvert.DeserializeObject<WarehouseEquipmentList>(smpResponse);
                return smpResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        
    }


}

