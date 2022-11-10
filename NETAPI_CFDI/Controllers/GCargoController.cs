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
    [RoutePrefix("api/GCargo")]
    public class GCargoController : ApiController
    {
        // GET: api/GCargo
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/GCargo/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/GCargo
        //public void Post([FromBody]string value)
        //{
        //}
        [HttpGet]
        [Route("Search")]
        
        public HttpResponseMessage Search([FromUri] GCSearchParam aobj_Param)
        {


            DataTable ldtb_Result = new DataTable();// ' la tabla que obtiene el resultado
            OleDbDataAdapter iAdapt_comand = new OleDbDataAdapter();
            OleDbCommand iolecmd_comand = new OleDbCommand();
            OleDbConnection ioleconx_conexion = new OleDbConnection();// '' objeto de conexion que se usara para conectar
                                                                      //ADODB.conection obj = new ADODB.conection();

            
            GCSearchResult retuobject = new GCSearchResult();

            string istr_conx = "";// ' cadena de conexion
            string strSQL = "";
            string lstr_data = "";
            istr_conx = System.Configuration.ConfigurationManager.ConnectionStrings["dbCalathus"].ToString();
            ioleconx_conexion.ConnectionString = istr_conx;
            iolecmd_comand = ioleconx_conexion.CreateCommand();

            ldtb_Result = new DataTable("User");
            strSQL = "spCRUDCFDIGCargo";

            iolecmd_comand.Parameters.Add("intMode", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("alngGCUniversal", OleDbType.Numeric);
            iolecmd_comand.Parameters.Add("aitem", OleDbType.Numeric);

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

            iolecmd_comand.Parameters["intMode"].Value = 1;
            iolecmd_comand.Parameters["alngGCUniversal"].Value = aobj_Param.lngGCUniversalId;
            iolecmd_comand.Parameters["aitem"].Value = 0;


            iolecmd_comand.Parameters["strContainer"].Value = "";
            iolecmd_comand.Parameters["ilngVesselVoyage"].Value = 0;
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


                    try
                    {
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
                    if (int.TryParse(ldtb_Result.Rows[0]["VOLUME"].ToString(), out valueint) == false)
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



    }

    public class GCSearchParam    {
        
        public long lngGCUniversalId { get; set; }

    }

    public class GCSearchResult
    {
        
        public string strProduct { get; set; }
        public bool blnIMO { get; set; }
        public bool blnSobreDim { get; set; }

    }


}
