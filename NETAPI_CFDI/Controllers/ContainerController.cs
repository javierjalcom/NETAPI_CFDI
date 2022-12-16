using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using System.Data.Odbc;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Data;


using Newtonsoft.Json;
using System.Text;

namespace NETAPI_CFDI.Controllers
{
    [RoutePrefix("api/Container")]
    public class ContainerController : ApiController
    {
        //// GET: api/Container
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Container/5
        //public string Get(int id)
        //{
        //    return "value";
        //}


        // GET: api/Container/5

        [HttpGet]
        [Route("Search")]
        //public string Search([FromUri] ContSearchParam aobj_Param)
        public HttpResponseMessage  Search([FromUri] ContSearchParam aobj_Param)
        {


            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

            ContSearchResult retuobject = new ContSearchResult();

            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spCRUDCFDIContainer";

            iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("strContainer", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("ilngVesselVoyage", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("ilngContUniversal", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strCFDIFolio", OleDbType.VarChar) ;
            iolecmd_comand.Parameters.Add("monCFDIPartial", OleDbType.Decimal);
            iolecmd_comand.Parameters.Add("monCFDIComplete", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("monCFDICompensation", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("intServiceCFDI", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strService", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intServOrderId", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strXParam", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intXParam", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("user", OleDbType.VarChar);

            iolecmd_comand.Parameters["intMode"].Value = 1;
            iolecmd_comand.Parameters["strContainer"].Value = aobj_Param.strContainer;
            iolecmd_comand.Parameters["ilngVesselVoyage"].Value = aobj_Param.lngVesselVoyage;
            iolecmd_comand.Parameters["ilngContUniversal"].Value = 0;

            iolecmd_comand.Parameters["strCFDIFolio"].Value = "";
            iolecmd_comand.Parameters["monCFDIPartial"].Value = 0;
            iolecmd_comand.Parameters["monCFDIComplete"].Value = 0;
            iolecmd_comand.Parameters["monCFDICompensation"].Value = 0;

            iolecmd_comand.Parameters["intServiceCFDI"].Value = 0;

            iolecmd_comand.Parameters["strService"].Value = "";
            iolecmd_comand.Parameters["intServOrderId"].Value = 0;

            iolecmd_comand.Parameters["strXParam"].Value = "";
            iolecmd_comand.Parameters["intXParam"].Value = 0;

            iolecmd_comand.Parameters["user"].Value = "";


            iolecmd_comand.CommandText = strSQL;
            iolecmd_comand.CommandType = CommandType.StoredProcedure;
            iolecmd_comand.CommandTimeout = 99999;

            try
            {
                iAdapt_comand.SelectCommand = iolecmd_comand;
                iAdapt_comand.Fill(ldtb_Result);

                if (ldtb_Result.Rows.Count == 1 && ldtb_Result.Columns.Count > 1)
                {
                    lstr_data = ldtb_Result.Rows[0][0].ToString();
                    
                    long valuelong = 0;
                    decimal valuedecimal = 0;
                    int valueint = 0;

                    if (long.TryParse(ldtb_Result.Rows[0]["intContainerUniversalId"].ToString(), out valuelong) == false)
                        valuelong = 0;
                    retuobject.lngContainerUniversalId = valuelong;
                    
                    if (decimal.TryParse(ldtb_Result.Rows[0]["decWeight"].ToString(), out valuedecimal) == false)
                         valuedecimal= 0;
                    retuobject.decWeight= valuedecimal;

                    try {
                        retuobject.strProduct = ldtb_Result.Rows[0]["strProduct"].ToString();
                    }
                    catch (Exception ex)
                    { }

                    //
                    valueint = 0;
                    if (int.TryParse(ldtb_Result.Rows[0]["IMO"].ToString(), out valueint) == false)
                        valueint = 0;

                    if (valueint > 0)
                        retuobject.blnIMO = true;
                    else
                        retuobject.blnIMO = false;


                    //
                    valueint = 0;
                    if (int.TryParse(ldtb_Result.Rows[0]["OverDim"].ToString(), out valueint) == false)
                        valueint = 0;

                    if (valueint > 0)
                        retuobject.blnSobreDim = true;
                    else
                        retuobject.blnSobreDim = false;


                }
            }
            catch (Exception ex)
            {
                string strError = ObtenerError(ex.Message, 99999);
                strError = ex.Message;
                //if (strError != null)
                //    return strError;
                //else
                //    return "0";
                //return Dt_RetrieveErrorTable(strError);
            }
            finally
            {
                ioleconx_conexion.Close();
            }
            // return ldtb_Result;
            //return JsonConvert.SerializeObject(retuobject);

            string ljson = JsonConvert.SerializeObject(retuobject);

            //var response = this.Request.CreateResponse(HttpStatusCode.OK);
            var response = this.Request.CreateResponse();




            response.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
            return response;

        }


        ///
        ///   [HttpGet]
        //[Route("Service")]
        //public string Search([FromUri] ContSearchParam aobj_Param)
        //public HttpResponseMessage Service([FromUri] ServiceSearchParam aobj_Param)
        //{


        //    DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
        //    OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
        //    OleDbCommand iolecmd_comand = new OleDbCommand();
        //    OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
        //                                                              //ADODB.conection obj = new ADODB.conection();

        //    List<ContitemResult> listContainer = new List<ContitemResult>();
        //    ContSearchResult retuobject = new ContSearchResult();

        //    string istr_conx = "";// ' cadena de conexion
        //    string strSQL = "";
        //    string lstr_data = "";
        //    istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
        //    ioleconx_conexion.ConnectionString = istr_conx;
        //    iolecmd_comand = ioleconx_conexion.CreateCommand();

        //    ldtb_Result = new DataTable("User");
        //    strSQL = "spCRUDCFDIContainer";

        //    iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
        //    iolecmd_comand.Parameters.Add("strContainer", OleDbType.VarChar);
        //    iolecmd_comand.Parameters.Add("ilngVesselVoyage", OleDbType.Numeric);
        //    iolecmd_comand.Parameters.Add("ilngContUniversal", OleDbType.Numeric);

        //    iolecmd_comand.Parameters.Add("strCFDIFolio", OleDbType.VarChar);
        //    iolecmd_comand.Parameters.Add("monCFDIPartial", OleDbType.Decimal);
        //    iolecmd_comand.Parameters.Add("monCFDIComplete", OleDbType.Decimal);

        //    iolecmd_comand.Parameters.Add("monCFDICompensation", OleDbType.Decimal);

        //    iolecmd_comand.Parameters.Add("intServiceCFDI", OleDbType.Numeric);

        //    iolecmd_comand.Parameters.Add("strService", OleDbType.VarChar);
        //    iolecmd_comand.Parameters.Add("intServOrderId", OleDbType.Numeric);

        //    iolecmd_comand.Parameters.Add("strXParam", OleDbType.VarChar);
        //    iolecmd_comand.Parameters.Add("intXParam", OleDbType.Numeric);

        //    iolecmd_comand.Parameters.Add("user", OleDbType.VarChar);

        //    iolecmd_comand.Parameters["intMode"].Value = 2;
        //    iolecmd_comand.Parameters["strContainer"].Value = "";
        //    iolecmd_comand.Parameters["ilngVesselVoyage"].Value = 0;
        //    iolecmd_comand.Parameters["ilngContUniversal"].Value = 0;

        //    iolecmd_comand.Parameters["strCFDIFolio"].Value = "";
        //    iolecmd_comand.Parameters["monCFDIPartial"].Value = 0;
        //    iolecmd_comand.Parameters["monCFDIComplete"].Value = 0;
        //    iolecmd_comand.Parameters["monCFDICompensation"].Value = 0;

        //    iolecmd_comand.Parameters["intServiceCFDI"].Value = 0;

        //    iolecmd_comand.Parameters["strService"].Value = aobj_Param.strService;
        //    iolecmd_comand.Parameters["intServOrderId"].Value = aobj_Param.lngServiceId;

        //    iolecmd_comand.Parameters["strXParam"].Value = "";
        //    iolecmd_comand.Parameters["intXParam"].Value = 0;

        //    iolecmd_comand.Parameters["user"].Value = "";


        //    iolecmd_comand.CommandText = strSQL;
        //    iolecmd_comand.CommandType = CommandType.StoredProcedure;
        //    iolecmd_comand.CommandTimeout = 99999;

        //    ContitemResult lobj_Contelement;
        //    try
        //    {
        //        iAdapt_comand.SelectCommand = iolecmd_comand;
        //        iAdapt_comand.Fill(ldtb_Result);

        //        if (ldtb_Result.Rows.Count > 0 && ldtb_Result.Columns.Count > 1)
        //        {
        //            foreach (DataRow lrow in ldtb_Result.Rows)
        //            {

        //                lobj_Contelement = new ContitemResult();

        //                long valuelong = 0;
        //                decimal valuedecimal = 0;
        //                int valueint = 0;

        //                if (long.TryParse(lrow["intContainerUniversalId"].ToString(), out valuelong) == false)
        //                    valuelong = 0;

        //                lobj_Contelement.lngContainerUniversalId = valuelong;

        //                if (decimal.TryParse(lrow["decWeight"].ToString(), out valuedecimal) == false)
        //                    valuedecimal = 0;

        //                lobj_Contelement.decWeight = valuedecimal;

        //                try
        //                {
        //                    lobj_Contelement.strProduct = lrow["strProduct"].ToString();
        //                    lobj_Contelement.strContainerId = lrow["strContainerId"].ToString();
        //                    // el lleno en strign 
        //                    lstr_data = lrow["isFull"].ToString();

        //                    if (lstr_data.ToLower() == "true" || lstr_data.ToLower() == "1")
        //                    {
        //                        lobj_Contelement.blnFull = true;
        //                    }
        //                    else
        //                    {
        //                        lobj_Contelement.blnFull = false;
        //                    }

        //                }
        //                catch (Exception ex)
        //                { }

        //                //
        //                valueint = 0;
        //                if (int.TryParse(lrow["IMO"].ToString(), out valueint) == false)
        //                    valueint = 0;

        //                if (valueint > 0)
        //                    lobj_Contelement.blnIMO = true;
        //                else
        //                    lobj_Contelement.blnIMO = false;


        //                //lectura sobredim
        //                valueint = 0;
        //                if (int.TryParse(lrow["OverDim"].ToString(), out valueint) == false)
        //                    valueint = 0;

        //                if (valueint > 0)
        //                    lobj_Contelement.blnSobreDim = true;
        //                else
        //                    lobj_Contelement.blnSobreDim = false;

        //                // llectura si es lleno 
        //                // valueint = 0;
        //                //if (int.TryParse(lrow["isFull"].ToString(), out valueint) == false)
        //                //  valueint = 0;

        //                //if (valueint > 0)
        //                //  lobj_Contelement.blnFull = true;
        //                //else
        //                //  lobj_Contelement.blnFull = false;

        //                ////////////////

        //                ///

        //                // si tiene universal ,agregar a ala lista 
        //                if (lobj_Contelement.lngContainerUniversalId > 0)
        //                {
        //                    listContainer.Add(lobj_Contelement);
        //                } //if  (lobj_Contelement.lngContainerUniversalId > 0)

        //            }  // foreach


        //        } //  if (ldtb_Result.Rows.Count >0 && ldtb_Result.Columns.Count > 1)
        //    }
        //    catch (Exception ex)
        //    {
        //        string strError = ObtenerError(ex.Message, 99999);
        //        strError = ex.Message;
        //        //if (strError != null)
        //        //    return strError;
        //        //else
        //        //    return "0";
        //        //return Dt_RetrieveErrorTable(strError);
        //    }
        //    finally
        //    {
        //        ioleconx_conexion.Close();
        //    }
        //    // return ldtb_Result;
        //    //return JsonConvert.SerializeObject(retuobject);

        //    string ljson = JsonConvert.SerializeObject(listContainer);

        //    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //    var response = this.Request.CreateResponse();
        //    response.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
        //    return response;

        //}
        //////


        ///
        [HttpGet]
        [Route("Service")]        
        public HttpResponseMessage Service([FromUri] ServiceSearchParam aobj_Param)
        {
            string lstr_service = "";
            lstr_service = aobj_Param.strService.ToUpper();

            ContMasterResultIN lobj_returnMasterIn = new ContMasterResultIN();
            ContMasterResultOUT lobj_returnMasterOUT = new ContMasterResultOUT();
            List<ContitemResult> listContainerIN = new List<ContitemResult>();
            
            List<ContitemResultFout> listContainerOUT = new List<ContitemResultFout>();
            List<ContitemResultYard> listContainerYARD = new List<ContitemResultYard>();


            string ljson = "";
                try
                   {
                     switch( lstr_service )
                      {
                    case "IN":
                                lobj_returnMasterIn= of_GetINServiceItems(aobj_Param);
                                ljson = JsonConvert.SerializeObject(lobj_returnMasterIn);

                                var responseIN = this.Request.CreateResponse(HttpStatusCode.OK);
                        
                                if (lobj_returnMasterIn.status =="OK")
                                    {
                                      responseIN = this.Request.CreateResponse(HttpStatusCode.OK);
                                   }

                                if (lobj_returnMasterIn.status == "NotFound")
                                    {
                                        responseIN = this.Request.CreateResponse(HttpStatusCode.NotFound);
                                    }

                                if (lobj_returnMasterIn.status == "BadRequest")
                                   {
                                     responseIN = this.Request.CreateResponse(HttpStatusCode.BadRequest);
                                }

                                responseIN.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
                                return responseIN;
                        break;

                    case "OUT":
                        lobj_returnMasterOUT = of_GetOUTServiceItems(aobj_Param);
                        ljson = JsonConvert.SerializeObject(lobj_returnMasterOUT);

                        var responseOUT = this.Request.CreateResponse(HttpStatusCode.OK);

                        if (lobj_returnMasterOUT.status == "OK")
                        {
                            responseOUT = this.Request.CreateResponse(HttpStatusCode.OK);
                        }

                        if (lobj_returnMasterOUT.status == "NotFound")
                        {
                            responseOUT = this.Request.CreateResponse(HttpStatusCode.NotFound);
                        }

                        if (lobj_returnMasterOUT.status == "BadRequest")
                        {
                            responseOUT = this.Request.CreateResponse(HttpStatusCode.BadRequest);
                        }

                        responseOUT.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
                        return responseOUT;

                        break;

                    case "YARD":
                        listContainerYARD = of_GetYARDServiceItems(aobj_Param);
                        ljson = JsonConvert.SerializeObject(listContainerOUT);
                        break;

                    default:
                        ljson = JsonConvert.SerializeObject(listContainerIN);
                        break;
                      }


                     
                   }
                catch (Exception ex)
                  {
                   string strError = ObtenerError(ex.Message, 99999);
                   strError = ex.Message;
                   ljson = "";
                   }
            finally
                 {
                     
                 }
            
            //var response = this.Request.CreateResponse(HttpStatusCode.OK);
            var response = this.Request.CreateResponse();
            response.Content = new StringContent(ljson, Encoding.UTF8, "application/json");
            return response;

        }

        //public List<ContitemResultFout> of_GetOUTServiceItems ( ServiceSearchParam aobj_Param)
        public ContMasterResultOUT of_GetOUTServiceItems(ServiceSearchParam aobj_Param)
        {
            ContMasterResultOUT lobj_return = new ContMasterResultOUT();

            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

            List<ContitemResultFout> listcontainer = new List<ContitemResultFout>();
            
            

            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spCRUDCFDIContainer";

            iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("strContainer", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("ilngVesselVoyage", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("ilngContUniversal", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strCFDIFolio", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("monCFDIPartial", OleDbType.Decimal);
            iolecmd_comand.Parameters.Add("monCFDIComplete", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("monCFDICompensation", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("intServiceCFDI", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strService", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intServOrderId", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strXParam", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intXParam", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("user", OleDbType.VarChar);

            iolecmd_comand.Parameters["intMode"].Value = 2;
            iolecmd_comand.Parameters["strContainer"].Value = "";
            iolecmd_comand.Parameters["ilngVesselVoyage"].Value = 0;
            iolecmd_comand.Parameters["ilngContUniversal"].Value = 0;

            iolecmd_comand.Parameters["strCFDIFolio"].Value = "";
            iolecmd_comand.Parameters["monCFDIPartial"].Value = 0;
            iolecmd_comand.Parameters["monCFDIComplete"].Value = 0;
            iolecmd_comand.Parameters["monCFDICompensation"].Value = 0;

            iolecmd_comand.Parameters["intServiceCFDI"].Value = 0;

            iolecmd_comand.Parameters["strService"].Value = aobj_Param.strService;
            iolecmd_comand.Parameters["intServOrderId"].Value = aobj_Param.lngServiceId;

            iolecmd_comand.Parameters["strXParam"].Value = "";
            iolecmd_comand.Parameters["intXParam"].Value = 0;

            iolecmd_comand.Parameters["user"].Value = "";


            iolecmd_comand.CommandText = strSQL;
            iolecmd_comand.CommandType = CommandType.StoredProcedure;
            iolecmd_comand.CommandTimeout = 99999;

            ContitemResultFout lobj_Contelement;
            try
            {
                iAdapt_comand.SelectCommand = iolecmd_comand;
                iAdapt_comand.Fill(ldtb_Result);

                if (ldtb_Result.Rows.Count > 0 && ldtb_Result.Columns.Count > 1)
                {
                    foreach (DataRow lrow in ldtb_Result.Rows)
                    {

                        lobj_Contelement = new ContitemResultFout();

                        long valuelong = 0;
                        decimal valuedecimal = 0;
                        int valueint = 0;

                        if (long.TryParse(lrow["intContainerUniversalId"].ToString(), out valuelong) == false)
                            valuelong = 0;

                        lobj_Contelement.lngContainerUniversalId = valuelong;

                        if (decimal.TryParse(lrow["decWeight"].ToString(), out valuedecimal) == false)
                            valuedecimal = 0;

                        lobj_Contelement.decWeight = valuedecimal;
                        //
                        valuedecimal = 0;
                        if (decimal.TryParse(lrow["decPercentage"].ToString(), out valuedecimal) == false)
                            valuedecimal = 0;

                        lobj_Contelement.decPercentage = valuedecimal;
                        lobj_return.fecha  = lrow["Fecha"].ToString();


                        try
                        {
                            lobj_Contelement.strProduct = lrow["strProduct"].ToString();
                            lobj_Contelement.strContainerId = lrow["strContainerId"].ToString();
                            lobj_Contelement.strType = lrow["strType"].ToString();
                            // el lleno en strign 
                            lstr_data = lrow["isFull"].ToString();

                            if (lstr_data.ToLower() == "true" || lstr_data.ToLower() == "1")
                            {
                                lobj_Contelement.blnFull = true;
                            }
                            else
                            {
                                lobj_Contelement.blnFull = false;
                            }

                        }
                        catch (Exception ex)
                        { }

                        //
                        valueint = 0;
                        if (int.TryParse(lrow["IMO"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnIMO = true;
                        else
                            lobj_Contelement.blnIMO = false;


                        //lectura sobredim
                        valueint = 0;
                        if (int.TryParse(lrow["OverDim"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnSobreDim = true;
                        else
                            lobj_Contelement.blnSobreDim = false;

                        

                        ///

                        // si tiene universal ,agregar a ala lista 
                        if (lobj_Contelement.lngContainerUniversalId > 0)
                        {
                            listcontainer.Add(lobj_Contelement);
                        } //if  (lobj_Contelement.lngContainerUniversalId > 0)

                    }  // foreach


                } //  if (ldtb_Result.Rows.Count >0 && ldtb_Result.Columns.Count > 1)
                if (ldtb_Result.Rows.Count == 0)
                    lobj_return.status = "NotFound";

                if (ldtb_Result.Rows.Count == 1 && ldtb_Result.Columns.Count == 1)
                    lobj_return.status = "BadRequest";


            }
            catch (Exception ex)
            {
                string strError = ObtenerError(ex.Message, 99999);
                strError = ex.Message;
                lobj_return.status = "BadRequest";
                //if (strError != null)
                //    return strError;
                //else
                //    return "0";
                //return Dt_RetrieveErrorTable(strError);
            }
            finally
            {
                ioleconx_conexion.Close();
            }
            // return ldtb_Result;
            //return JsonConvert.SerializeObject(retuobject);

            try
            {
                if (listcontainer.Count > 0)
                {
                    lobj_return.datos = new List<ContitemResultFout>();
                    lobj_return.datos = listcontainer;
                    lobj_return.status = "OK";
                }
            }
            catch (Exception ex)
            {
                lobj_return.datos = new List<ContitemResultFout>();
                lobj_return.status = "NotFound";
            }

            return lobj_return;
            
        }
        ////
        ///
        //public List<ContitemResult> of_GetINServiceItems(ServiceSearchParam aobj_Param)
         public ContMasterResultIN of_GetINServiceItems(ServiceSearchParam aobj_Param)
        {
            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

            ContMasterResultIN lobj_return = new ContMasterResultIN();

            List<ContitemResult> listcontainer = new List<ContitemResult>();



            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spCRUDCFDIContainer";

            iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("strContainer", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("ilngVesselVoyage", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("ilngContUniversal", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strCFDIFolio", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("monCFDIPartial", OleDbType.Decimal);
            iolecmd_comand.Parameters.Add("monCFDIComplete", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("monCFDICompensation", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("intServiceCFDI", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strService", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intServOrderId", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strXParam", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intXParam", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("user", OleDbType.VarChar);

            iolecmd_comand.Parameters["intMode"].Value = 2;
            iolecmd_comand.Parameters["strContainer"].Value = "";
            iolecmd_comand.Parameters["ilngVesselVoyage"].Value = 0;
            iolecmd_comand.Parameters["ilngContUniversal"].Value = 0;

            iolecmd_comand.Parameters["strCFDIFolio"].Value = "";
            iolecmd_comand.Parameters["monCFDIPartial"].Value = 0;
            iolecmd_comand.Parameters["monCFDIComplete"].Value = 0;
            iolecmd_comand.Parameters["monCFDICompensation"].Value = 0;

            iolecmd_comand.Parameters["intServiceCFDI"].Value = 0;

            iolecmd_comand.Parameters["strService"].Value = aobj_Param.strService;
            iolecmd_comand.Parameters["intServOrderId"].Value = aobj_Param.lngServiceId;

            iolecmd_comand.Parameters["strXParam"].Value = "";
            iolecmd_comand.Parameters["intXParam"].Value = 0;

            iolecmd_comand.Parameters["user"].Value = "";


            iolecmd_comand.CommandText = strSQL;
            iolecmd_comand.CommandType = CommandType.StoredProcedure;
            iolecmd_comand.CommandTimeout = 99999;

            ContitemResult lobj_Contelement;
            try
            {
                iAdapt_comand.SelectCommand = iolecmd_comand;
                iAdapt_comand.Fill(ldtb_Result);

                if (ldtb_Result.Rows.Count > 0 && ldtb_Result.Columns.Count > 1)
                {
                    foreach (DataRow lrow in ldtb_Result.Rows)
                    {

                        lobj_Contelement = new ContitemResult();

                        long valuelong = 0;
                        decimal valuedecimal = 0;
                        int valueint = 0;


                        if (long.TryParse(lrow["intContainerUniversalId"].ToString(), out valuelong) == false)
                            valuelong = 0;

                        lobj_Contelement.lngContainerUniversalId = valuelong;

                        if (decimal.TryParse(lrow["decWeight"].ToString(), out valuedecimal) == false)
                            valuedecimal = 0;

                        lobj_Contelement.decWeight = valuedecimal;
                        lobj_return.fecha = lrow["Fecha"].ToString();

                        try
                        {
                            lobj_Contelement.strProduct = lrow["strProduct"].ToString();
                            lobj_Contelement.strContainerId = lrow["strContainerId"].ToString();
                            lobj_Contelement.strType  = lrow["strType"].ToString();
                            // el lleno en strign 
                            lstr_data = lrow["isFull"].ToString();

                            if (lstr_data.ToLower() == "true" || lstr_data.ToLower() == "1")
                            {
                                lobj_Contelement.blnFull = true;
                            }
                            else
                            {
                                lobj_Contelement.blnFull = false;
                            }
                            

                        }
                        catch (Exception ex)
                        { }

                        //
                        valueint = 0;
                        if (int.TryParse(lrow["IMO"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnIMO = true;
                        else
                            lobj_Contelement.blnIMO = false;


                        //lectura sobredim
                        valueint = 0;
                        if (int.TryParse(lrow["OverDim"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnSobreDim = true;
                        else
                            lobj_Contelement.blnSobreDim = false;



                        ///
                        //lobj_Contelement.status = "OK";

                        // si tiene universal ,agregar a ala lista 
                        if (lobj_Contelement.lngContainerUniversalId > 0)
                        {
                            listcontainer.Add(lobj_Contelement);
                        } //if  (lobj_Contelement.lngContainerUniversalId > 0)
                    }  // foreach




                } //  if (ldtb_Result.Rows.Count >0 && ldtb_Result.Columns.Count > 1)

                if (ldtb_Result.Rows.Count == 0 )
                    lobj_return.status = "NotFound";

                if (ldtb_Result.Rows.Count == 1 && ldtb_Result.Columns.Count ==1)
                    lobj_return.status = "BadRequest";
            }
            catch (Exception ex)
            {
                string strError = ObtenerError(ex.Message, 99999);
                strError = ex.Message;
                lobj_return.status = "BadRequest";
                //if (strError != null)
                //    return strError;
                //else
                //    return "0";
                //return Dt_RetrieveErrorTable(strError);
            }
            finally
            {
                ioleconx_conexion.Close();
            }
            // return ldtb_Result;
            //return JsonConvert.SerializeObject(retuobject);

            try
            {
                if (listcontainer.Count > 0)
                {
                    lobj_return.datos = new List<ContitemResult>();
                    lobj_return.datos = listcontainer;
                    lobj_return.status = "OK";
                }
            }
            catch (Exception ex)
            {
                lobj_return.datos = new List<ContitemResult>();                
                lobj_return.status = "NotFound";
            }
            return lobj_return;

        }
        ///

        ///
        ///
        public List<ContitemResultYard> of_GetYARDServiceItems(ServiceSearchParam aobj_Param)
        {
            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

            List<ContitemResultYard> listcontainer = new List<ContitemResultYard>();



            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spCRUDCFDIContainer";

            iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("strContainer", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("ilngVesselVoyage", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("ilngContUniversal", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strCFDIFolio", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("monCFDIPartial", OleDbType.Decimal);
            iolecmd_comand.Parameters.Add("monCFDIComplete", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("monCFDICompensation", OleDbType.Decimal);

            iolecmd_comand.Parameters.Add("intServiceCFDI", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strService", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intServOrderId", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("strXParam", OleDbType.VarChar);
            iolecmd_comand.Parameters.Add("intXParam", OleDbType.Numeric);

            iolecmd_comand.Parameters.Add("user", OleDbType.VarChar);

            iolecmd_comand.Parameters["intMode"].Value = 3;
            iolecmd_comand.Parameters["strContainer"].Value = "";
            iolecmd_comand.Parameters["ilngVesselVoyage"].Value = 0;
            iolecmd_comand.Parameters["ilngContUniversal"].Value = 0;

            iolecmd_comand.Parameters["strCFDIFolio"].Value = "";
            iolecmd_comand.Parameters["monCFDIPartial"].Value = 0;
            iolecmd_comand.Parameters["monCFDIComplete"].Value = 0;
            iolecmd_comand.Parameters["monCFDICompensation"].Value = 0;

            iolecmd_comand.Parameters["intServiceCFDI"].Value = 0;

            iolecmd_comand.Parameters["strService"].Value = aobj_Param.strService;
            iolecmd_comand.Parameters["intServOrderId"].Value = aobj_Param.lngServiceId;

            iolecmd_comand.Parameters["strXParam"].Value = "";
            iolecmd_comand.Parameters["intXParam"].Value = 0;

            iolecmd_comand.Parameters["user"].Value = "";


            iolecmd_comand.CommandText = strSQL;
            iolecmd_comand.CommandType = CommandType.StoredProcedure;
            iolecmd_comand.CommandTimeout = 99999;

            ContitemResultYard lobj_Contelement;
            try
            {
                iAdapt_comand.SelectCommand = iolecmd_comand;
                iAdapt_comand.Fill(ldtb_Result);

                if (ldtb_Result.Rows.Count > 0 && ldtb_Result.Columns.Count > 1)
                {
                    foreach (DataRow lrow in ldtb_Result.Rows)
                    {

                        lobj_Contelement = new ContitemResultYard();

                        long valuelong = 0;
                        decimal valuedecimal = 0;
                        int valueint = 0;

                        if (long.TryParse(lrow["intContainerUniversalId"].ToString(), out valuelong) == false)
                            valuelong = 0;

                        lobj_Contelement.lngContainerUniversalId = valuelong;

                        if (decimal.TryParse(lrow["decWeight"].ToString(), out valuedecimal) == false)
                            valuedecimal = 0;

                        lobj_Contelement.decWeight = valuedecimal;


                        try
                        {
                            lobj_Contelement.strProduct = lrow["strProduct"].ToString();
                            lobj_Contelement.strContainerId = lrow["strContainerId"].ToString();
                            // el lleno en strign 
                            lstr_data = lrow["isFull"].ToString();

                            if (lstr_data.ToLower() == "true" || lstr_data.ToLower() == "1")
                            {
                                lobj_Contelement.blnFull = true;
                            }
                            else
                            {
                                lobj_Contelement.blnFull = false;
                            }

                        }
                        catch (Exception ex)
                        { }

                        //
                        valueint = 0;
                        if (int.TryParse(lrow["IMO"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnIMO = true;
                        else
                            lobj_Contelement.blnIMO = false;


                        //lectura sobredim
                        valueint = 0;
                        if (int.TryParse(lrow["OverDim"].ToString(), out valueint) == false)
                            valueint = 0;

                        if (valueint > 0)
                            lobj_Contelement.blnSobreDim = true;
                        else
                            lobj_Contelement.blnSobreDim = false;



                        ///

                        // si tiene universal ,agregar a ala lista 
                        if (lobj_Contelement.lngContainerUniversalId > 0)
                        {
                            listcontainer.Add(lobj_Contelement);
                        } //if  (lobj_Contelement.lngContainerUniversalId > 0)

                    }  // foreach


                } //  if (ldtb_Result.Rows.Count >0 && ldtb_Result.Columns.Count > 1)
            }
            catch (Exception ex)
            {
                string strError = ObtenerError(ex.Message, 99999);
                strError = ex.Message;
                //if (strError != null)
                //    return strError;
                //else
                //    return "0";
                //return Dt_RetrieveErrorTable(strError);
            }
            finally
            {
                ioleconx_conexion.Close();
            }
            // return ldtb_Result;
            //return JsonConvert.SerializeObject(retuobject);

            return listcontainer;

        }

        ///
        public string ObtenerError(String cad, int ex)
        {

            if ((cad.Contains(ex.ToString()) == true) && (cad.Contains("Sybase Provider]") == true))
            {

                int idx = cad.LastIndexOf("]");
                idx = idx + 1;

                if ((idx > 0) && (idx <= cad.Length))
                    return cad.Substring(idx);
                else
                    return "";

            }
            else
            {
                if (cad.Contains("SSybase Provider]") == true)
                {
                    int idx;
                    idx = cad.LastIndexOf("]");
                    idx = idx + 1;

                    if (idx > 0 && idx <= cad.Length)
                        return cad.Substring(idx);
                    else
                        return "";
                }

            } // else if((cad.Contains(ex.ToString()) == True) &&(cad.Contains("Sybase Provider]") == True))

            return "";

        }

        // POST: api/Container
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT: api/Container/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/Container/5
        //public void Delete(int id)
        //{
        //}


        public class ContSearchParam
        {
            public string strContainer { get; set; }
            public long lngVesselVoyage { get; set; }

        }

        public class ContSearchYARDParam
        {
            public string strContainer { get; set; }
            public long lngServiceorderId { get; set; }

        }
        public class ContSearchResult
        {
            public long lngContainerUniversalId { get; set; }
            public decimal decWeight { get; set; }
            public string strProduct { get; set; }
            public bool blnIMO { get; set; }
            public bool blnSobreDim { get; set; }

        }

        public class ContitemResult
        {
            public long lngContainerUniversalId { get; set; }
            public string strContainerId { get; set; }
            public decimal decWeight { get; set; }
            public string strProduct { get; set; }
            public bool blnIMO { get; set; }
            public bool blnSobreDim { get; set; }
            public bool blnFull { get; set; }
            public string strType { get; set; }

        }

        public class ContMasterResultIN
        {
            public string status { get; set; }
            public string fecha { get; set; }
            public IList<ContitemResult> datos { get; set; }
        }

        public class ContMasterResultOUT
        {
            public string status { get; set; }
            public string fecha { get; set; }
            public IList<ContitemResultFout> datos { get; set; }
        }
        public class ContitemResultFout
        {
            public long lngContainerUniversalId { get; set; }
            public string strContainerId { get; set; }
            public decimal decWeight { get; set; }
            public string strProduct { get; set; }
            public bool blnIMO { get; set; }
            public bool blnSobreDim { get; set; }
            public bool blnFull { get; set; }
            public decimal decPercentage { get; set; }
            public string strType { get; set; }
        }

        public class ContitemResultYard
        {
            public long lngContainerUniversalId { get; set; }
            public string strContainerId { get; set; }
            public decimal decWeight { get; set; }
            public string strProduct { get; set; }
            public bool blnIMO { get; set; }
            public bool blnSobreDim { get; set; }
            public bool blnFull { get; set; }
            public decimal decPercentage { get; set; }
        }

        public class ServiceSearchParam
        {
            public string strService{ get; set; }
            public long lngServiceId { get; set; }
            
        }
    }
}
