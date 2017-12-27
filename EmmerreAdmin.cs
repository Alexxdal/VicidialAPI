using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
using System.Net;

namespace Emmerre_Admin
{
    class EmmerreAdmin
    {
        private String ipaddress;
        private String username;
        private String userpass;
        private CookieAwareWebClient webclient = new CookieAwareWebClient();
        //Array di elementi
        public String[] Campagne = new String[150];//Campagne ID and Name STD;Campagna unica
        public String[] CampagneDettaglio = new String[150];//campagnaID;campagnaNome;campagnaStato;campagnaDialMode;
        public String[] CampagnaSettings = new String[150];//jsondata.Split(",".ToCharArray());  MOLTO DETTAGLIATO
        public Series PointSeries = new Series();//SERIE PUNTI GRAFICO
        public String[] ReportAgentDetail = new String[150]; //GetAgentStatusDetail();
        public String[] Lists = new String[200];//ListID;ListName;ListStatus;ListLastCall;ListLenght;ListCampaign
        public String ListDetail;
        public String[] ExcelFirstRow = new String[30];
        public String[] Agents = new String[400];
        public String[] AgentDetails;
        public String[] RealTimeAgentsStats = new String[1000];
        public String[] RealTimeAgentsStatsDetail = new String[500];
        public String[] AgentGroups = new String[50];
        public String[] SearchResults = new String[150];
        //Status Agenti vars Block
        public String AgentiInChiamata;
        public String AgentiInPausa;
        public String AgentiInAttesa;
        public String AgentiInLinea;
        public String[] UserInfoResults = new String[500];
        public String[] AgentTalkTime = new String[1000];
        public String[] AgentLoginLogout = new String[1000];
        public String[] AgentOutboundCalls = new String[1000];
        public String[] AgentManualDialCalls = new String[1000];
        public String[] AgentRecs = new String[1000];
        //CallStatus vars Block
        public String ChiamateInAttesa;
        public String CodaChiamateInUscita;
        public String CodaChiamateInEntrata;
        public String ChiamateInEntrata;
        public String ChiamateInUscita;
        public String ChiamateTotali;
        public String ChiamateRisposte;
        public String NumeriDisponibiliDaChiamare;
        public String NumeriTotaliTutteListe;
        public String PercentualeSaltate;
        public String ChiamateSaltate;
        //Server Info
        public String Hostname;
        public String ListeningIP;
        public String KernelVersion;
        public String DistroName;
        public String Uptime;
        public String LoadAverages;
        public String PhysicalMemoryUsed;
        public String TotalDiskUsage;
        //Sippy Info
        public String Balance;
        //Vendite Giornaliere
        public String VenditeTotali = String.Empty;
        public String VenditeInUscita = String.Empty;
        public String VenditeInIngresso = String.Empty;
        //Variabili Liste
        public String LastListId = String.Empty;
        public String[] ListEsitiArr;




        //Costruttore
        /// <summary>
        /// Crea nuovo oggetto.
        /// </summary>
        /// <param name="_ip">Indirizzo del server</param>
        /// <param name="_user">Username</param>
        /// <param name="_pass">Password</param>
        public EmmerreAdmin(String _ip, String _user, String _pass)
        {
            ipaddress = _ip;
            username = _user;
            userpass = _pass;
        }
        //Function to Login
        public void Login()
        {
            webclient.BaseAddress = @"https://" + ipaddress;
            // establish login data
            var loginData = new NameValueCollection
            {
                { "user_name", username },
                { "user_pass", userpass }
            };
            // begin login
            webclient.UploadValues("/index.php/go_login/validate_credentials", "POST", loginData);
        }
        //Get VEndite
        //VenditeTotali; VenditeInIngresso; VenditeInUscita
        public void GetSalesToday()
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_sales_today").Replace("\r", "").Replace("\t", "");
            String[] sourceArray = source.Split("\n".ToCharArray());
            VenditeTotali = sourceArray[5].Replace("<td class=\"b\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            VenditeInIngresso = sourceArray[9].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            VenditeInUscita = sourceArray[13].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
        }
        //Get info to calls
        public void GetStatistiche()
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_calls_today").Replace("\t", "");
            //AgentiInChiamata
            String[] sourcearray = source.Split("\n".ToCharArray());
            ChiamateTotali = sourcearray[63].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInAttesa = sourcearray[20].Replace("<td class=\"o dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            CodaChiamateInUscita = sourcearray[27].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            CodaChiamateInEntrata = sourcearray[36].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer;\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInEntrata = sourcearray[44].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            ChiamateInUscita = sourcearray[51].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"callMonitoring()\" title=\"Click to see calls being placed\">", "").Replace("</a></div></div></td>", "");
            //Numeritotali
            String source2 = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_leads").Replace("\t", "");
            String[] sourcearray2 = source2.Split("\n".ToCharArray());
            NumeriDisponibiliDaChiamare = sourcearray2[37].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            NumeriTotaliTutteListe = sourcearray2[41].Replace("<td class=\"c\"><a class=\"cur_hand\">", "").Replace("</a></td>", "");
            //Chiamaterispo e percentuale
            String source3 = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_drops_today").Replace("\t", "");
            String[] sourcearray3 = source3.Split("\n".ToCharArray());
            PercentualeSaltate = sourcearray3[20].Replace("<td class=\"o dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer;font-size:50px;\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign dropped percentage\">", "").Replace("</a></div></div></td>", "");
            ChiamateSaltate = sourcearray3[28].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign dropped calls\">", "").Replace("</a></div></div></td>", "");
            ChiamateRisposte = sourcearray3[36].Replace("<td class=\"c dropTD\"><div class=\"tdcon1\"><div class=\"tdcon2\"><a class=\"toolTip\" style=\"cursor:pointer\" onclick=\"droppedCalls()\" title=\"Click to see the list of campaign answered calls\">", "").Replace("</a></div></div></td>", "");
        }
        //Get SippyInfo
        public void GetSippyInfo()
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/sippyinfo");
            int startindex = source.IndexOf("payWithPayPalbalance");
            int bal0 = source.IndexOf("</a>", startindex);
            int bal1 = source.LastIndexOf("\">", bal0);
            Balance = source.Substring(bal1 + 2, bal0 - (bal1 + 2));
        }
        //Return Points Series for Chart
        public void GetChartData(String daData, String aData, String campagna)
        {
            try
            {
                //Get Points Data
                webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_reports_output/stats/" + daData + "/" + aData + "/" + campagna + "/daily/");
                //Get Points Data JSON
                String pointsData = webclient.DownloadString("https://" + ipaddress + "/data/stats-daily-ADMIN.json");
                int index1 = pointsData.IndexOf("[[");
                int index2 = pointsData.IndexOf("]]");
                pointsData = pointsData.Substring(index1 + 2, index2 - (index1 + 2)).Replace("],[", "\n").Replace("\"", "");
                String[] PointsArray = pointsData.Split("\n".ToCharArray());
                //Settings DataPointsCollection
                for (int i = 0; i < PointsArray.Length; i++)
                {
                    String[] values = PointsArray[i].Split(",".ToCharArray());
                    PointSeries.Points.AddXY(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]));
                }
            }
            catch (Exception) { return; }
        }






        //#######################################################################################################################################
        //##############################################        GESTIONE AGENTI             #####################################################
        //Function to get Agent Status
        //AgentiInChiamata; AgentiInPausa; AgentiInAttesa; AgentiInLinea
        public void GetAgentStatus()
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_dashboard_agents").Replace("\t", "");
            //AgentiInChiamata
            String[] sourcearray = source.Split("\n".ToCharArray());
            AgentiInChiamata = sourcearray[5].Replace("<td class=\"b\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInPausa = sourcearray[9].Replace("<td class=\"c\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInAttesa = sourcearray[13].Replace("<td class=\"c\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
            AgentiInLinea = sourcearray[17].Replace("<td class=\"b\"><a class=\"cur_hand toolTip\" style=\"cursor:pointer\" onclick=\"agentMonitoring()\" title=\"Click to monitor agents\">", "").Replace("</a></td>", "");
        }
        //Dettaglio Agenti
        //ReportAgentDetail[];
        public void GetAgentStatusDetail(String daData, String aData, String campagna)
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_reports_output/agent_pdetail/" + daData + "/" + aData + "/" + campagna).Replace("\t", "").Replace("\r", "");
            //Check if there are results in here
            if (source.Contains("Nessun agente trovato in questo periodo"))
            {
                ReportAgentDetail[0] = "Nessun agente trovato in questo periodo";
                return;
            }

            int agentdetailStart = source.IndexOf("<!-- Start Agent Performance Detail -->");
            int agentdetailEnd = source.IndexOf("<!-- End Agent Performance Detail -->");
            String AgentDetail = source.Substring(agentdetailStart, agentdetailEnd - agentdetailStart);
            AgentDetail = AgentDetail.Remove(0, AgentDetail.IndexOf("<tr style=\"background-color:#E0F8E0;\">") + 38);
            AgentDetail = AgentDetail.Remove(AgentDetail.IndexOf("</table>")).Replace("</tr><tr style=\"background-color:#EFFBEF;\">", "@").Replace("</tr><tr style=\"background-color:#E0F8E0;\">", "@");
            AgentDetail = AgentDetail.Replace("        ", "").Replace("</tr>      <tr style=\"background-color:#FFFFFF;\">", "@").Replace("left", "right");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:dashed 1px #D0D0D0;\"><div align=\"right\" class=\"style4\" style=\"font-size: 10px;\">&nbsp; ", ";").Replace(" &nbsp;</div></td>", "");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:#D0D0D0 dashed 1px;\"><div align=\"right\" class=\"style4\" style=\"font-size:10px\"><b>TOTALE</b></div></td>", "");
            AgentDetail = AgentDetail.Replace("<td nowrap style=\"border-top:#D0D0D0 dashed 1px;\"><div align=\"right\" class=\"style4\" style=\"font-size:10px\">&nbsp; ", ";");
            AgentDetail = AgentDetail.Replace("</strong> ", "").Replace("<strong>", "").Replace("      </tr>", "").Replace("\n", "");
            String[] AgentArray = AgentDetail.Split("@".ToCharArray());
            for (int i = 0; i < AgentArray.Length; i++)
            {
                ReportAgentDetail[i] = AgentArray[i].Remove(0, 1);
            }
        }
        public void GetAgents()
        {
            int page = 1;
            int agentindex = 0;
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/1").Replace("\t", "").Replace("\r", "");
            while (source.Contains("user-cols-container"))
            {
                //Get source for this page
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page).Replace("\t", "").Replace("\r", "");
                //Modify Source
                int user0 = source.IndexOf("user-tbl-rows");
                int user1 = source.IndexOf("</tbody>", user0);
                source = source.Substring(user0 + 16, user1 - (user0 + 16));
                source = source.Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\">&nbsp;&nbsp;", "");
                source = source.Replace("<a class='action-id toolTip' style='cursor:pointer' title='Modify user ", "");
                source = source.Replace("<span style=\"color:blu;font-weight:bold;\">", "").Replace("<tr style=\"background-color:#c3e1ff;\" class='user-tbl-rows'>", "").Replace("<tr style=\"background-color:#65a2df;\" class='user-tbl-rows'>", "");
                //Get Text With only one agent
                for (int i = 0; source.IndexOf("user-action-modify-") > 0; i++)
                {
                    int agent0 = source.IndexOf(">");
                    int agent1 = source.IndexOf("</span></td>", agent0);
                    String data = source.Substring(agent0 + 1, agent1 - (agent0 + 1));
                    //Get Data
                    int agid1 = data.IndexOf("</a></td>");
                    String AgentId = data.Substring(0, agid1);
                    //Adjust remaining data of agent
                    data = data.Replace("</a></td>", ";").Replace("</td>", ";").Replace("\n", "");
                    //Another step
                    int trim0 = data.IndexOf(">");
                    data = AgentId + ";" + data.Substring(trim0 + 1, data.Length - (trim0 + 1));
                    //ADD DATA TO ARRAY
                    Agents[agentindex] = data;
                    //Remove this agent from source
                    agent1 = source.IndexOf("</tr>");
                    source = source.Substring(agent1 + 5, source.Length - (agent1 + 5));
                    //Agent index increment
                    agentindex++;
                }
                page++;
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page).Replace("\t", "").Replace("\r", "");
            }
        }//Tutti gli agenti in Agents[]   ID;nome;livello;gruppo;stato
        public void AgentsLogout(String CampaignID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "campaign", CampaignID }
            };
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_campaign_ce/emergencylogout", "POST", reqparm);
        }//Slogga tutti gli agenti in una campagna
        public void AgentLogout(String AgentID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "user", AgentID }
            };
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/emergencylogout", "POST", reqparm);
        }//Slogga il singolo agente
        //Get Numeric agent ID from String ID
        public String GetAgentID(String agentName)
        {
            int page = 1;
            String agent = agentName;
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page + "/" + agent).Replace("\t", "").Replace("\r", "");
            //Check if agent was found in page 1
            while (!source.Contains("user-action-modify-"))
            {
                source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/index/search/" + page + "/" + agent).Replace("\t", "").Replace("\r", "");
                page++;
            }
            //Get Agent ID
            int id0 = source.IndexOf("rel='");
            int id1 = source.IndexOf("'", id0 + 6);
            return source.Substring(id0 + 5, id1 - (id0 + 5));
        }
        //Get Agent group
        public String GetAgentGroup(String agentNameID)
        {
            GetAgentSettings(agentNameID);
            String gruppo = AgentDetails[5].Split(":".ToCharArray())[1];
            return gruppo;
        }
        public void GetAgentGroups()
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("https://" + ipaddress + "/_vicidial_/realtime_report.php");
            int gro0 = source.IndexOf("var select_list = '");
            int gro1 = source.IndexOf(";", gro0);
            source = source.Substring(gro0 + 19, gro1 - (gro0 + 19));
            //Adjuctments
            gro0 = source.IndexOf("<SELECT SIZE=8 NAME=user_group_filter[] ID=user_group_filter[] multiple>");
            gro1 = source.IndexOf("</SELECT>", gro0 + 20);
            source = source.Substring(gro0 + 72, gro1 - (gro0 + 72));
            source = source.Replace("<option selected value=\"", "").Replace("</option><option value=\"", ";").Replace("</option>", "").Replace("\">", ",");
            //Create array of group
            AgentGroups = source.Split(";".ToCharArray());
            AgentGroups = AgentGroups.Where(c => c != null).ToArray();
        }
        //Get Setting for specific agent
        public void GetAgentSettings(String agentID)
        {
            String ID = GetAgentID(agentID);
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_user_ce/collectuserinfo/" + ID);
            //Collect info
            source = source.Replace("}]", "").Replace("[{", "").Replace("\"", "").Replace(",", "\n");
            AgentDetails = source.Split("\n".ToCharArray());
        }
        //Cambia il nome dell'agente
        public void ChangeAgentName(String agentID, String newName)
        {
            //Get agent Settings
            GetAgentSettings(agentID);

            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            String pass = AgentDetails[2].Split(":".ToCharArray())[1];
            String ID = GetAgentID(agentID);
            String fullname = AgentDetails[3].Split(":".ToCharArray())[1];
            String phone_login = AgentDetails[6].Split(":".ToCharArray())[1];
            String phone_pass = AgentDetails[7].Split(":".ToCharArray())[1];
            String user_group = AgentDetails[5].Split(":".ToCharArray())[1];
            String active = AgentDetails[58].Split(":".ToCharArray())[1];
            String hotkeys_active = AgentDetails[20].Split(":".ToCharArray())[1];
            String user_level = AgentDetails[4].Split(":".ToCharArray())[1];
            String modify_same_user_level = AgentDetails[95].Split(":".ToCharArray())[1];
            //Set new name
            fullname = newName;
            //Set Params
            reqparm.Add("users_id-" + agentID, agentID);
            reqparm.Add("vicidial_user_id-" + agentID, ID);
            reqparm.Add(agentID + "-pass", pass);
            reqparm.Add(agentID + "-full_name", fullname);
            reqparm.Add("phone_login-" + agentID, phone_login);
            reqparm.Add("phone_pass-" + agentID, phone_pass);
            reqparm.Add("user_group-" + agentID, user_group);
            reqparm.Add("active-" + agentID, active);
            reqparm.Add("hotkeys_active-" + agentID, hotkeys_active);
            reqparm.Add("user_level-" + agentID, user_level);
            reqparm.Add("modify_same_user_level-" + agentID, modify_same_user_level);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/updateuser/" + ID + "/" + agentID, "POST", reqparm);
        }
        //Change agent password
        public void ChangeAgentPass(String agentID, String newpass)
        {
            //Get agent Settings
            GetAgentSettings(agentID);

            var reqparm = new System.Collections.Specialized.NameValueCollection();
            //VARS
            String pass = AgentDetails[2].Split(":".ToCharArray())[1];
            String ID = GetAgentID(agentID);
            String fullname = AgentDetails[3].Split(":".ToCharArray())[1];
            String phone_login = AgentDetails[6].Split(":".ToCharArray())[1];
            String phone_pass = AgentDetails[7].Split(":".ToCharArray())[1];
            String user_group = AgentDetails[5].Split(":".ToCharArray())[1];
            String active = AgentDetails[58].Split(":".ToCharArray())[1];
            String hotkeys_active = AgentDetails[20].Split(":".ToCharArray())[1];
            String user_level = AgentDetails[4].Split(":".ToCharArray())[1];
            String modify_same_user_level = AgentDetails[95].Split(":".ToCharArray())[1];
            //Set new pass
            pass = newpass;
            //Set Params
            reqparm.Add("users_id-" + agentID, agentID);
            reqparm.Add("vicidial_user_id-" + agentID, ID);
            reqparm.Add(agentID + "-pass", pass);
            reqparm.Add(agentID + "-full_name", fullname);
            reqparm.Add("phone_login-" + agentID, phone_login);
            reqparm.Add("phone_pass-" + agentID, phone_pass);
            reqparm.Add("user_group-" + agentID, user_group);
            reqparm.Add("active-" + agentID, active);
            reqparm.Add("hotkeys_active-" + agentID, hotkeys_active);
            reqparm.Add("user_level-" + agentID, user_level);
            reqparm.Add("modify_same_user_level-" + agentID, modify_same_user_level);
            //Send Request
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/updateuser/" + ID + "/" + agentID, "POST", reqparm);
        }
        public void GetRealTimeAgents(String _group, String _campaign)
        {
            String gruppo = _group;
            String campagna = _campaign;
            //Get REal Time Agents Data
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_site/go_monitoring/0/timeup/agents/" + gruppo + "/" + campagna);
            //Substring useful data
            int data0 = source.IndexOf("</th></tr></thead><tbody><tr style=\"color:#333\" align=center>");
            int data1 = source.IndexOf("</tr></tbody>");
            source = source.Substring(data0 + 62, data1 - (data0 + 63));
            //Adjust new data
            source = source.Replace(" id=\"trid\"", "").Replace("display: none;color:#333;", "color:#333").Replace("<td nowrap style=\"font-size:11px;\">&nbsp;", "").Replace("</span>&nbsp;", "").Replace("&nbsp;", "");
            source = source.Replace("<td nowrap style=\"font-size:11px;cursor:pointer;\" class=\"toolTip\" title=\"Click to listen or barge:<br />", "").Replace("\"><span id=\"sendMonitor\" onclick=\"sendMonitor('", "").Replace("','" + ipaddress + "');\">", "§");
            source = source.Replace("<td nowrap style=\"font-size:11px;background-color:", "").Replace(";color:black;\" >", ",").Replace(";color:white;\" >", ",");
            source = source.Replace("<span style=\"cursor:pointer\" onclick=\"modify('", "").Replace("')\">", ",").Replace("<span>", "");
            source = source.Replace("</tr><tr style=\"color:#333\" align=center>", "@");
            source = source.Replace("  ", "").Replace("\n", "");
            //Put Data in to array
            String[] _realtimedata = source.Split("@".ToCharArray());
            //Ultimi aggiustamenti
            for (int i = 0; i < _realtimedata.Length; i++)
            {
                int uno = _realtimedata[i].IndexOf("§");
                int due = _realtimedata[i].Length;
                _realtimedata[i] = _realtimedata[i].Substring(uno + 1, due - (uno + 1));
            }
            //Finally Put DataS in Array
            RealTimeAgentsStats = _realtimedata;
        }
        public void GetRealTimeAgentsDetail(String campagna, String gruppo)
        {
            //VARS
            String RTajax = "1";
            String DB = "0";
            String groups = "ALL-ACTIVE";
            String user_group_filter = "ALL-GROUPS";
            String adastats = "2";
            String SIPmonitorLINK = "";
            String IAXmonitorLINK = "";
            String usergroup = "";
            String UGdisplay = "0";
            String UidORname = "1";
            String orderby = "timeup";
            String SERVdisplay = "0";
            String CALLSdisplay = "1";
            String PHONEdisplay = "0";
            String CUSTPHONEdisplay = "0";
            String with_inbound = "Y";
            String monitor_active = "";
            String monitor_phone = "";
            String ALLINGROUPstats = "";
            String DROPINGROUPstats = "0";
            String NOLEADSalert = "";
            String CARRIERstats = "0";
            String PRESETstats = "0";
            String AGENTtimeSTATS = "0";
            //Settings Parameter
            groups = campagna;
            user_group_filter = gruppo;

            webclient.Credentials = new NetworkCredential(username, userpass);
            //#########
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "RTajax", RTajax },
                { "DB", DB },
                { "groups[]", groups },
                { "user_group_filter[]", user_group_filter },
                { "adastats", adastats },
                { "SIPmonitorLINK", SIPmonitorLINK },
                { "IAXmonitorLINK", IAXmonitorLINK },
                { "usergroup", usergroup },
                { "UGdisplay", UGdisplay },
                { "UidORname", UidORname },
                { "orderby", orderby },
                { "SERVdisplay", SERVdisplay },
                { "CALLSdisplay", CALLSdisplay },
                { "PHONEdisplay", PHONEdisplay },
                { "CUSTPHONEdisplay", CUSTPHONEdisplay },
                { "with_inbound", with_inbound },
                { "monitor_active", monitor_active },
                { "monitor_phone", monitor_phone },
                { "ALLINGROUPstats", ALLINGROUPstats },
                { "DROPINGROUPstats", DROPINGROUPstats },
                { "NOLEADSalert", NOLEADSalert },
                { "CARRIERstats", CARRIERstats },
                { "PRESETstats", PRESETstats },
                { "AGENTtimeSTATS", AGENTtimeSTATS }
            };
            //Decode response byte
            String source = new ASCIIEncoding().GetString(webclient.UploadValues("https://" + ipaddress + "/_vicidial_/AST_timeonVDADall.php", "POST", reqparm));
            //Remove Unuseful data from source
            int _zero = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------") + 5;
            int zero = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------", _zero);
            int uno = source.IndexOf("+----------------+------------------------+-----------+-----------------+---------+------------+-------+------+------------------", zero + 10);
            //Get Clean String
            source = source.Substring(zero + 130, uno - (zero + 131));
            //Adjust Data
            String[] _Agenti = source.Split("\n".ToCharArray());
            for (int i = 0; i < _Agenti.Length; i++)
            {
                _Agenti[i] = _Agenti[i].Replace("      </B></SPAN> ", "").Replace(" <SPAN class=\"", "").Replace("<SPAN class=\"", "").Replace(" <a href=\"./user_status.php?user=", "").Replace("\" target=\"_blank\">", "|").Replace("\"><B>", "|");
                _Agenti[i] = _Agenti[i].Replace("</B></SPAN></a> <a href=\"javascript:ingroup_info('", "|").Replace("','" + i + "');\">+</a>", "").Replace("  </B></SPAN> ", "").Replace(" </B></SPAN>          ", "").Replace("</B></SPAN> ", "");
                //Remove Blank chars
                _Agenti[i] = _Agenti[i].Replace("        ", "").Replace("       ", "").Replace("    ", "").Replace("</a> <a href=\"javascript:ingroup_info('", "");
                _Agenti[i] = _Agenti[i].Substring(1, _Agenti[i].Length - 1);
            }
            //Put Results to Array
            RealTimeAgentsStatsDetail = _Agenti;
        }
        //Ascolto/intrusione agente 
        public void AscoltoAgente(String palmarino, String AgentSessionID)
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_barged_in/BARGE/" + AgentSessionID + "/" + palmarino + "/" + ipaddress);
        }
        public void UserInfo(String AgentID)//Function used in 'Dettaglio Agente'; UserInfoResults[];
        {
            var reqparm = new NameValueCollection
            {
                { "userid", AgentID }
            };
            //Decode response byte
            String source = new ASCIIEncoding().GetString(webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/userinfo", "POST", reqparm));
            //Do Things
            String[] json = source.Split(",".ToCharArray());
            for (int i = 0; i < json.Length; i++)
            {
                json[i] = json[i].Replace("\"", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
            }
            UserInfoResults = json;
        }
        //DETTAGLIO AGENTE---------------------
        public void AgentTalkTimeStatus(String agentID, String dadata, String adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=1");
            source = source.Replace("\"", "");
            String[] finaldata = source.Split("\n".ToCharArray());
            AgentTalkTime = finaldata;
        }
        public void AgentLoginLogoutStatus(String agentID, String dadata, String adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=2");
            source = source.Replace("\"", "");
            String[] finaldata = source.Split("\n".ToCharArray());
            AgentLoginLogout = finaldata;
        }
        public void AgentOutboundsCallsStatus(String agentID, String dadata, String adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=5");
            source = source.Replace("\"", "");
            String[] finaldata = source.Split("\n".ToCharArray());
            AgentOutboundCalls = finaldata;
        }
        public void AgentManualDialStatus(String agentID, String dadata, String adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=9");
            source = source.Replace("\"", "");
            String[] finaldata = source.Split("\n".ToCharArray());
            AgentManualDialCalls = finaldata;
        }
        public void AgentRecStatus(String agentID, String dadata, String adata)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            String source = webclient.DownloadString("http://" + ipaddress + "/_vicidial_/user_stats.php?&begin_date=" + dadata + "&end_date=" + adata + "&user=" + agentID + "&submit=submit&file_download=8");
            source = source.Replace("\"", "");
            String[] finaldata = source.Split("\n".ToCharArray());
            AgentRecs = finaldata;
        }
        //-------------------------------------
        public void AddAgent(String _nome, String _full_name, String _password, String _gruppo)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "group1-accountNum", _gruppo },
                { "group1-user", _nome },
                { "group1-pass", _password },
                { "group1-full_name", _full_name },
                { "group1-active", "Y" }
            };
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/autogenuser", "POST", reqparm);
        }
        //Check agent format "agent001" if exist, "1" == no
        public String CheckAgentDuplicate(String _agentid)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "user", _agentid }
            };
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/duplicate", "POST", reqparm);
            String response = new ASCIIEncoding().GetString(responsebytes);
            return response;
        }
        public String GetNewUserAutoCompileByGroup(String _gruppo)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "accountNum", _gruppo },
                { "hidcount", "1" },
                { "txtSeats", "1" },
                { "skip", "skip" },
                { "generate_phone", "0" },
                { "start_phone_exten", "" }
            };
            byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_user_ce/userwizard", "POST", reqparm);
            String response = new ASCIIEncoding().GetString(responsebytes);
            //grab necessary data
            int ind0 = response.IndexOf("\"right\":\"");
            int ind1 = response.IndexOf("form>", ind0);
            response = response.Substring(ind0 + 9, ind1 - (ind0 + 9));
            response = response.Replace("\"", "").Replace(" ", "").Replace("\\", "");
            //getnome
            int nom0 = response.IndexOf("group1-uservalue=") + 17;
            int nom1 = response.IndexOf("id", nom0);
            String idagente = response.Substring(nom0, nom1 - nom0);
            //getpass
            int pass0 = response.IndexOf("group1-passvalue=") + 17;
            int pass1 = response.IndexOf("id", pass0);
            String pass = response.Substring(pass0, pass1 - pass0);
            //getfullname
            int fnam0 = response.IndexOf("group1-full_namevalue=") + 22;
            int fname1 = response.IndexOf("id", fnam0);
            String full_name = response.Substring(fnam0, fname1 - fnam0);

            return idagente + "," + pass + "," + full_name;
        }



        //#########################################################################################################################################
        //##############################################        GESTIONE CAMPAGNE             #####################################################
        //Set Campagne Array with data
        public void GetCampagne()
        {
            int CurIndex = 0;
            //Clear global array
            String source = webclient.DownloadString("https://" + ipaddress + "/reports");
            //Find first id
            int index1 = source.IndexOf("<div id=\"campaign_ids\" class=\"go_campaign_menu\">");
            int index2 = source.IndexOf("</div>", index1);
            //Create array
            String[] campaignArray = source.Substring(index1, index2 - index1).Replace("\t", "").Split("\n".ToCharArray());
            //Extract data
            for (int i = 0; i < campaignArray.Length; i++)
            {
                if (campaignArray[i].Contains("<li class=\"go_campaign_submenu\" style=\"padding: 3px 10px 3px 3px; margin: 0px; white-space: nowrap;\" title=\""))
                {
                    Campagne[CurIndex] = campaignArray[i].Replace("<li class=\"go_campaign_submenu\" style=\"padding: 3px 10px 3px 3px; margin: 0px; white-space: nowrap;\" title=\"", "").Replace("\">", ";").Replace("</li>", "");
                    CurIndex++;
                }
            }
        }
        public void GetCampagneDetiail()
        {
            try
            {
                String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_campaign_list");
                //Get Table HTML
                int table0 = source.IndexOf("<table id=\"mainTable\" class=\"tablesorter\" border=\"0\" cellpadding=\"1\" cellspacing=\"0\" style=\"width:100%;\">");
                int table1 = source.IndexOf("</table>", table0);
                String table = source.Substring(table0, table1 - table0);
                //Get TBODY
                int tbody0 = table.IndexOf("<tbody>");
                int tbody1 = table.IndexOf("</tbody>");
                String tbody = table.Substring(tbody0, tbody1 - tbody0);
                //Get Number Of Campaign
                int campaignCount = 0;
                int index = 0;
                while (tbody.IndexOf("<tr", index) != -1)
                {
                    if (tbody.IndexOf("<tr", index) != -1)
                    {
                        campaignCount++;
                        index = tbody.IndexOf("<tr", index) + 10;
                    }
                }
                index = 0;
                for (int i = 0; i < campaignCount; i++)
                {
                    if (tbody.IndexOf("<tr", index) != -1)
                    {
                        int tr1 = tbody.IndexOf("<tr", index);
                        int trsub = tbody.IndexOf(";\">", tr1);
                        int tr2 = tbody.IndexOf("/tr>", tr1);
                        index = tr1 + 5;
                        String tr = tbody.Substring(trsub + 4, tr2 - (trsub + 6)).Replace("&nbsp;", "").Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\">", "");
                        tr = tr.Replace("<td style=\"border-top:#D0D0D0 dashed 1px;\"", "").Replace("&#150;", "-");
                        String[] trArray = tr.Split("\n".ToCharArray());
                        //Extract Data from Array
                        //Get Campagna ID
                        int campid0 = trArray[0].IndexOf("<span onclick=\"modify('");
                        int campid1 = trArray[0].IndexOf("')\"");
                        String campagnaID = trArray[0].Substring(campid0 + 23, campid1 - (campid0 + 23));
                        //Get Nome Campagna
                        int campnam0 = trArray[1].IndexOf("\">");
                        int campnam1 = trArray[1].IndexOf("</span></td>");
                        String campagnaNome = trArray[1].Substring(campnam0 + 2, campnam1 - (campnam0 + 2));
                        //Get Campagna Stato
                        String campagnaStato = trArray[3].Replace("<span style=\"color:green;font-weight:bold;\">", "").Replace("<span style=\"color:red;font-weight:bold;\">", "").Replace("</span></td>", "").Replace("<span style=\"color:#F00;font-weight:bold;\">", "");
                        //Get Dial Metod
                        String campagnaDialMode = trArray[2].Replace("</td>", "");
                        CampagneDettaglio[i] = campagnaID + ";" + campagnaNome + ";" + campagnaStato + ";" + campagnaDialMode;
                    }
                }
            }
            catch (Exception) { }
            //remove null from array
            CampagneDettaglio = CampagneDettaglio.Where(c => c != null).ToArray();
        }
        public void CancellaCampagna(String CampagnaID)
        {
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_update_campaign_list/delete/" + CampagnaID);
            //Reload Campaign
        }
        public void GetCampagnaSettings(String campagnaID)
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_get_settings/" + campagnaID);
            int json0 = source.IndexOf("var testVar = jQuery.parseJSON('{");
            int json1 = source.IndexOf("}');", json0);
            String jsondata = source.Substring(json0 + 33, json1 - (json0 + 33)).Replace("\"", "");
            CampagnaSettings = jsondata.Split(",".ToCharArray());
        }
        public void ChangeCampagnaStatus(String CampagnaID, bool Stato)
        {
            String NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            String DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            String AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            String CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            String NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (String.IsNullOrEmpty(NumeroInUscita)) { NumeroInUscita = ""; }
            String CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            String CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            String CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            String CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            String DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            String CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            if (Stato == true) { CampagnaActive = "Y"; } else { CampagnaActive = "N"; }
            if (String.IsNullOrEmpty(CampagnaActive)) { CampagnaActive = "Y"; }
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaNome(String CampagnaID, String Nome)
        {
            String NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            String DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            String AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            String CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            String NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            String CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            String CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            String CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            String CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            String DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            String CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            NomeCampagna = Nome;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaVelocita(String CampagnaID, String Velocita)
        {
            String NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            String DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            String AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            String CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            String NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (String.IsNullOrEmpty(NumeroInUscita)) { NumeroInUscita = ""; }
            String CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            String CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            String CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            String CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            String DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            String CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            AutoDialLevel = Velocita;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }
        public void ChangeCampagnaSegreteria(String CampagnaID, String Segreteria)
        {
            String NomeCampagna = CampagnaSettings[0].Split(":".ToCharArray())[1];
            String DialMethod = CampagnaSettings[3].Split(":".ToCharArray())[1];
            String AutoDialLevel = CampagnaSettings[4].Split(":".ToCharArray())[1];
            String CampaignScript = CampagnaSettings[5].Split(":".ToCharArray())[1];
            if (CampaignScript == "null") { CampaignScript = ""; }
            String NumeroInUscita = CampagnaSettings[6].Split(":".ToCharArray())[1];
            if (String.IsNullOrEmpty(NumeroInUscita)) { NumeroInUscita = ""; }
            String CampaignRecording = CampagnaSettings[7].Split(":".ToCharArray())[1];
            String CampaignSegreteria = CampagnaSettings[9].Split(":".ToCharArray())[1];
            String CampaignLocalTime = CampagnaSettings[10].Split(":".ToCharArray())[1];
            String CampaignDescription = CampagnaSettings[1].Split(":".ToCharArray())[1];
            String DialPrefix = CampagnaSettings[43].Split(":".ToCharArray())[1];
            String CampagnaActive = CampagnaSettings[15].Split(":".ToCharArray())[1];
            CampaignSegreteria = Segreteria;
            webclient.DownloadString("https://" + ipaddress + "/index.php/go_campaign_ce/go_modify_settings/" + CampagnaID + "/modify/" + NomeCampagna + "," + DialMethod + ",ADVANCE," + AutoDialLevel + "," + CampaignScript + "," + NumeroInUscita + "," + CampaignRecording + "," + CampaignSegreteria + "," + CampaignLocalTime + "," + CampaignDescription + ",,CUSTOM_" + DialPrefix + "," + CampagnaActive + "//0/////");
        }






        //#########################################################################################################################################
        //##############################################           GESTIONE LISTE             #####################################################
        public void GetListe()//ListID + ";" + ListName + ";" + ListStatus + ";" + ListLastCall + ";" + ListLenght + ";" + ListCampaign;
        {
            String source = webclient.DownloadString("https://" + ipaddress + "/go_list");
            int listind0 = source.IndexOf("<!-- LISTs TAB -->");
            int listind1 = source.IndexOf("<!-- end view -->");
            String Liste = source.Substring(listind0 + 18, listind1 - (listind0 + 18)).Replace("\r", "").Replace("\t", "").Replace(" ", "");

            //Try different pages
            int page = 1;
            int resultindex = -1;
            while (!Liste.Contains("Norecord(s)found!"))
            {
                source = webclient.DownloadString("https://" + ipaddress + "/go_list/go_list/lists/" + page);
                listind0 = source.IndexOf("<!-- LISTs TAB -->");
                listind1 = source.IndexOf("<!-- end view -->");
                Liste = source.Substring(listind0 + 18, listind1 - (listind0 + 18)).Replace("\r", "").Replace("\t", "").Replace(" ", "");
                //Cicle for get data
                for (int count = 0; count != Liste.Length || count != -1; count = Liste.IndexOf("<!--<divclass=\"rightdivtoolTip\"title=\"MODIFY", count + 20))
                {
                    if (count == -1) { break; }
                    if (count != 0)
                    {
                        int listid1 = Liste.IndexOf("\">-->", count + 20);
                        //Get ListID
                        String ListID = Liste.Substring(count + 44, listid1 - (count + 44));
                        //Get List Name
                        int listn0 = Liste.IndexOf("<tdcolspan=\"\"style=\"padding-bottom:-1px;\">\n", count);
                        int listn1 = Liste.IndexOf("</td>", listn0);
                        String ListName = Liste.Substring(listn0 + 44, listn1 - (listn0 + 44));
                        //Get List Status
                        int liststat0 = Liste.IndexOf("<b><fontcolor=", count);
                        int liststat1 = Liste.IndexOf(">", liststat0 + 10);
                        String ListStatus = Liste.Substring(liststat0 + 14, liststat1 - (liststat0 + 14));
                        if (ListStatus == "red") { ListStatus = "NON ATTIVA"; } else { ListStatus = "ATTIVA"; }
                        //Get List Last Call
                        int lastcall0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\">", liststat1);
                        int lastcall1 = Liste.IndexOf("</td>", lastcall0);
                        String ListLastCall = Liste.Substring(lastcall0 + 45, lastcall1 - (lastcall0 + 45));
                        if (ListLastCall == "&nbsp;") { ListLastCall = "Nessuna"; }
                        ListLastCall = ListLastCall.Replace("&#150;", "-").Replace("&nbsp;", "");
                        //Get List Number Lenght
                        int listlen0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\"><fontcolor=\"RED\"><b>", count);
                        int listlen1 = Liste.IndexOf("</b>", listlen0 + 60);
                        String ListLenght = Liste.Substring(listlen0 + 64, listlen1 - (listlen0 + 64));
                        //Get List Campaign
                        int listcam0 = Liste.IndexOf("<tdalign=\"left\"style=\"padding-bottom:-1px;\">", listlen1);
                        int listcam1 = Liste.IndexOf("&nbsp;</td>", listcam0);
                        String ListCampaign = Liste.Substring(listcam0 + 44, listcam1 - (listcam0 + 44));

                        //Set Array with results
                        Lists[resultindex] = ListID + ";" + ListName + ";" + ListStatus + ";" + ListLastCall + ";" + ListLenght + ";" + ListCampaign;
                    }
                    resultindex++;
                }
                page++;
            }
            Lists = Lists.Where(c => c != null).ToArray();
            if(Lists.Length > 0)
            {
                LastListId = Lists[Lists.Length - 1].Split(";".ToCharArray())[0];
            }
        }
        //Get List Detailed Information
        public void GetListDetails(String ListaID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "items", "showval=" + ListaID },
                { "action", "editlist" }
            };
            Byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editview", "POST", reqparm);
            String source = ASCIIEncoding.ASCII.GetString(responsebytes);
            source = source.Substring(0, source.IndexOf("##"));
            String[] sourceArray = source.Split(new String[] { "--" }, StringSplitOptions.None);
            //Get List Detail
            String ListID = sourceArray[1];
            String ListName = sourceArray[2];
            String ListCampaign = sourceArray[3];
            String ListStatus = sourceArray[4];
            String ListDescr = sourceArray[5];
            String DateNow = sourceArray[6];
            String ListLastCall = sourceArray[7];
            String ListScadenza = sourceArray[8];
            String ListCID = sourceArray[10];
            String Num1 = sourceArray[13];
            String Num2 = sourceArray[14];
            String Num3 = sourceArray[15];
            String Num4 = sourceArray[16];
            String Num5 = sourceArray[17];
            String ListWebForm = sourceArray[18];
            ListDetail = ListaID + ";" + ListName + ";" + ListCampaign + ";" + ListStatus + ";" + ListDescr + ";" + DateNow + ";" + ListLastCall + ";" + ListScadenza + ";" + ListCID + ";" + Num1 + ";" + Num2 + ";" + Num3 + ";" + Num4 + ";" + Num5 + ";" + ListWebForm;
        }
        public void ChangeListStatus(String ListaID, String status)
        {
            GetListDetails(ListaID);
            String[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            String ListID = ListaID;
            String ListName = Settings[1];
            String ListCampaign = Settings[2];
            String ListStatus = Settings[3];
            String ListDescr = Settings[4];
            String ListScadenza = Settings[7];
            String ListCID = Settings[8];
            String Num1 = Settings[9];
            String Num2 = Settings[10];
            String Num3 = Settings[11];
            String Num4 = Settings[12];
            String Num5 = Settings[13];
            String ListWebForm = Settings[14];
            String agent_script_override = "";
            String drop_inbound_group_override = "";
            String resetList = "N";
            //Active/Deactive
            ListStatus = status;
            String parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit", "POST", reqparm);
        }
        public void ResettaLista(String ListaID)
        {
            GetListDetails(ListaID);
            String[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            String ListID = ListaID;
            String ListName = Settings[1];
            String ListCampaign = Settings[2];
            String ListStatus = Settings[3];
            String ListDescr = Settings[4];
            String ListScadenza = Settings[7];
            String ListCID = Settings[8];
            String Num1 = Settings[9];
            String Num2 = Settings[10];
            String Num3 = Settings[11];
            String Num4 = Settings[12];
            String Num5 = Settings[13];
            String ListWebForm = Settings[14];
            String agent_script_override = "";
            String drop_inbound_group_override = "";
            String resetList = "Y";//Resetta Esiti
            String parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit", "POST", reqparm);
        }
        public void ChangeListCampaign(String ListaID, String CampaignID)
        {
            if (CampaignID.Contains(";")) { CampaignID = CampaignID.Split(";".ToCharArray())[0]; }
            GetListDetails(ListaID);
            String[] Settings = ListDetail.Split(";".ToCharArray());
            //Add Parameter to POST DATA
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            String ListID = ListaID;
            String ListName = Settings[1];
            String ListCampaign = Settings[2];
            String ListStatus = Settings[3];
            if (String.IsNullOrEmpty(ListStatus)) { ListStatus = "Y"; }
            String ListDescr = Settings[4];
            String ListScadenza = Settings[7];
            String ListCID = Settings[8];
            String Num1 = Settings[9];
            String Num2 = Settings[10];
            String Num3 = Settings[11];
            String Num4 = Settings[12];
            String Num5 = Settings[13];
            String ListWebForm = Settings[14];
            String agent_script_override = "";
            String drop_inbound_group_override = "";
            String resetList = "N";//Resetta Esiti no
            ListCampaign = CampaignID;
            String parametri1 = "editlist=editlist&editval=&showvaledit=" + ListaID + "&list_name=" + ListName + "&list_description=" + ListDescr + "&campaign_id=" + ListCampaign + "&reset_time=" + ListScadenza + "&reset_list=" + resetList + "&active=" + ListStatus + "&agent_script_override=" + agent_script_override + "&campaign_cid_override=" + ListCID + "&drop_inbound_group_override=" + drop_inbound_group_override + "&web_form_address=" + ListWebForm + "&xferconf_a_number=" + Num1 + "&xferconf_d_number=" + Num4 + "&xferconf_b_number=" + Num2 + "&xferconf_e_number=" + Num5 + "&xferconf_c_number=" + Num3;
            reqparm.Add("itemsumit", parametri1);
            reqparm.Add("action", "editlistfinal");
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editsubmit", "POST", reqparm);
        }
        public void CaricaNumeri(String _filepath, String _phone_number, String _first_name, String _last_name, String _address1, String _city, String _state, String _province, String _postal_code, String _alt_phone, String _email, String _comments, String _dupcheck)
        {
            //GET MIME TYPE
            String mimeType = "application/unknown";
            String ext = System.IO.Path.GetExtension(_filepath).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();


            webclient.SetTimeout(99999);
            //Data Vars Settings
            String boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            String ContentTypeBlank = "\r\n";
            String lineTerm = "\r\n";
            var fileData = webclient.Encoding.GetString(System.IO.File.ReadAllBytes(_filepath));
            String leadsloadVal = "ok";
            String tabvalselVal = "";
            String leadfile_nameVal = @"C:\fakepath\lista pulita" + ext;
            String list_id_overrideVal = LastListId;//Required
            String phone_code_overrideVal = "39";//Required
            String dupcheckVal = _dupcheck;//Required, check duplicates in listID
            String postalgmtVal = "AREA";
            String submit_fileVal = "CARICA NUMERI";
            String vendor_lead_code_fieldVal = "-1";
            String source_id_fieldVal = "-1";
            String phone_number_fieldVal = _phone_number;
            String title_fieldVal = "-1";
            String first_name_fieldVal = _first_name;
            String middle_initial_fieldVal = "-1";
            String last_name_fieldVal = _last_name;
            String address1_fieldVal = _address1;
            String address2_fieldVal = "-1";
            String address3_fieldVal = "-1";
            String city_fieldVal = _city;
            String state_fieldVal = _state;
            String province_fieldVal = _province;
            String postal_code_fieldVal = _postal_code;
            String country_code_fieldVal = "-1";
            String gender_fieldVal = "-1";
            String date_of_birth_fieldVal = "-1";
            String alt_phone_fieldVal = _alt_phone;
            String email_fieldVal = _email;
            String security_phrase_fieldVal = "-1";
            String comments_fieldVal = _comments;
            String rank_fieldVal = "-1";
            String owner_fieldVal = "-1";


            //Data to send
            String leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + leadsloadVal + lineTerm + "--";
            //----------
            String tabvalselReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"tabvalsel\"" + lineTerm + ContentTypeBlank + tabvalselVal + lineTerm + "--";
            //----------
            String leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            //----------
            String filedataString = fileData;
            String fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"; filename=\"lista pulita" + ext + "\"" + lineTerm + "Content-Type: " + mimeType + "\r\n\r\n" + filedataString + lineTerm + "--";
            //----------
            String list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            //----------
            String phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            //----------
            String dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            //----------
            String postalgmtReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postalgmt\"" + lineTerm + ContentTypeBlank + postalgmtVal + lineTerm + "--";
            //----------
            String submit_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"submit_file\"" + lineTerm + ContentTypeBlank + submit_fileVal + lineTerm + "--";


            //Setting Header Request
            webclient.Headers.Add("Accept", "*/*");
            webclient.Headers.Add("Accept-Language", "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3");
            webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            webclient.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Uri uri = new Uri("http://" + ipaddress + "/go_list");
            String data = "--" + leadsloadReq + tabvalselReq + leadfile_nameReq + fileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + postalgmtReq + submit_fileReq + boundary + "--\r\n";
            var datacollection = webclient.Encoding.GetBytes(data);
            var result = webclient.UploadData(uri, "POST", datacollection);



            //SECONDA FASE DEL CARICAMENTO
            boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            //---Settings data Part
            leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + "okfinal" + lineTerm + "--";
            String lead_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"lead_file\"" + lineTerm + ContentTypeBlank + "/tmp/admin_listapulita.txt" + lineTerm + "--";
            String leadfileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"" + lineTerm + ContentTypeBlank + "Array" + lineTerm + "--";
            list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            String superfinalReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"superfinal\"" + lineTerm + ContentTypeBlank + lineTerm + "--";
            String vendor_lead_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"vendor_lead_code_field\"" + lineTerm + ContentTypeBlank + vendor_lead_code_fieldVal + lineTerm + "--";
            String source_id_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"source_id_field\"" + lineTerm + ContentTypeBlank + source_id_fieldVal + lineTerm + "--";
            String phone_number_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_number_field\"" + lineTerm + ContentTypeBlank + phone_number_fieldVal + lineTerm + "--";
            String title_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"title_field\"" + lineTerm + ContentTypeBlank + title_fieldVal + lineTerm + "--";
            String first_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"first_name_field\"" + lineTerm + ContentTypeBlank + first_name_fieldVal + lineTerm + "--";
            String middle_initial_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"middle_initial_field\"" + lineTerm + ContentTypeBlank + middle_initial_fieldVal + lineTerm + "--";
            String last_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"last_name_field\"" + lineTerm + ContentTypeBlank + last_name_fieldVal + lineTerm + "--";
            String address1_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address1_field\"" + lineTerm + ContentTypeBlank + address1_fieldVal + lineTerm + "--";
            String address2_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address2_field\"" + lineTerm + ContentTypeBlank + address2_fieldVal + lineTerm + "--";
            String address3_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address3_field\"" + lineTerm + ContentTypeBlank + address3_fieldVal + lineTerm + "--";
            String city_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"city_field\"" + lineTerm + ContentTypeBlank + city_fieldVal + lineTerm + "--";
            String state_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"state_field\"" + lineTerm + ContentTypeBlank + state_fieldVal + lineTerm + "--";
            String province_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"province_field\"" + lineTerm + ContentTypeBlank + province_fieldVal + lineTerm + "--";
            String postal_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postal_code_field\"" + lineTerm + ContentTypeBlank + postal_code_fieldVal + lineTerm + "--";
            String country_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"country_code_field\"" + lineTerm + ContentTypeBlank + country_code_fieldVal + lineTerm + "--";
            String gender_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"gender_field\"" + lineTerm + ContentTypeBlank + gender_fieldVal + lineTerm + "--";
            String date_of_birth_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"date_of_birth_field\"" + lineTerm + ContentTypeBlank + date_of_birth_fieldVal + lineTerm + "--";
            String alt_phone_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"alt_phone_field\"" + lineTerm + ContentTypeBlank + alt_phone_fieldVal + lineTerm + "--";
            String email_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"email_field\"" + lineTerm + ContentTypeBlank + email_fieldVal + lineTerm + "--";
            String security_phrase_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"security_phrase_field\"" + lineTerm + ContentTypeBlank + security_phrase_fieldVal + lineTerm + "--";
            String comments_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"comments_field\"" + lineTerm + ContentTypeBlank + comments_fieldVal + lineTerm + "--";
            String rank_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"rank_field\"" + lineTerm + ContentTypeBlank + rank_fieldVal + lineTerm + "--";
            String owner_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"owner_field\"" + lineTerm + ContentTypeBlank + owner_fieldVal + lineTerm + "--";
            String OK_to_processReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"OK_to_process\"" + lineTerm + ContentTypeBlank + "PROCEDI" + lineTerm + "--";
            //Complete Data String
            data = "--" + leadsloadReq + lead_fileReq + leadfileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + leadfile_nameReq + superfinalReq + vendor_lead_code_fieldReq + source_id_fieldReq + phone_number_fieldReq + title_fieldReq + first_name_fieldReq + middle_initial_fieldReq + last_name_fieldReq + address1_fieldReq + address2_fieldReq + address3_fieldReq + city_fieldReq + state_fieldReq + province_fieldReq + postal_code_fieldReq + country_code_fieldReq + gender_fieldReq + date_of_birth_fieldReq + alt_phone_fieldReq + email_fieldReq + security_phrase_fieldReq + comments_fieldReq + rank_fieldReq + owner_fieldReq + OK_to_processReq + boundary + "--\r\n";
            datacollection = webclient.Encoding.GetBytes(data);
            result = webclient.UploadData(uri, "POST", datacollection);
            webclient.SetTimeout(0);
        }
        public void CaricaNumeri(String _listid, String _filepath, String _phone_number, String _first_name, String _last_name, String _address1, String _city, String _state, String _province, String _postal_code, String _alt_phone, String _email, String _comments, String _dupcheck)
        {
            //GET MIME TYPE
            String mimeType = "application/unknown";
            String ext = System.IO.Path.GetExtension(_filepath).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();

            webclient.SetTimeout(99999);
            //Data Vars Settings
            String boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            String ContentTypeBlank = "\r\n";
            String lineTerm = "\r\n";
            var fileData = webclient.Encoding.GetString(System.IO.File.ReadAllBytes(_filepath));
            String leadsloadVal = "ok";
            String tabvalselVal = "";
            String leadfile_nameVal = @"C:\fakepath\lista pulita" + ext;
            String list_id_overrideVal = _listid;//Required
            String phone_code_overrideVal = "39";//Required
            String dupcheckVal = _dupcheck;//Required, check duplicates in listID
            String postalgmtVal = "AREA";
            String submit_fileVal = "CARICA NUMERI";
            String vendor_lead_code_fieldVal = "-1";
            String source_id_fieldVal = "-1";
            String phone_number_fieldVal = _phone_number;
            String title_fieldVal = "-1";
            String first_name_fieldVal = _first_name;
            String middle_initial_fieldVal = "-1";
            String last_name_fieldVal = _last_name;
            String address1_fieldVal = _address1;
            String address2_fieldVal = "-1";
            String address3_fieldVal = "-1";
            String city_fieldVal = _city;
            String state_fieldVal = _state;
            String province_fieldVal = _province;
            String postal_code_fieldVal = _postal_code;
            String country_code_fieldVal = "-1";
            String gender_fieldVal = "-1";
            String date_of_birth_fieldVal = "-1";
            String alt_phone_fieldVal = _alt_phone;
            String email_fieldVal = _email;
            String security_phrase_fieldVal = "-1";
            String comments_fieldVal = _comments;
            String rank_fieldVal = "-1";
            String owner_fieldVal = "-1";


            //Data to send
            String leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + leadsloadVal + lineTerm + "--";
            //----------
            String tabvalselReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"tabvalsel\"" + lineTerm + ContentTypeBlank + tabvalselVal + lineTerm + "--";
            //----------
            String leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            //----------
            String filedataString = fileData;
            String fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"; filename=\"lista pulita" + ext + "\"" + lineTerm + "Content-Type: " + mimeType + "\r\n\r\n" + filedataString + lineTerm + "--";
            //----------
            String list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            //----------
            String phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            //----------
            String dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            //----------
            String postalgmtReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postalgmt\"" + lineTerm + ContentTypeBlank + postalgmtVal + lineTerm + "--";
            //----------
            String submit_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"submit_file\"" + lineTerm + ContentTypeBlank + submit_fileVal + lineTerm + "--";


            //Setting Header Request
            webclient.Headers.Add("Accept", "*/*");
            webclient.Headers.Add("Accept-Language", "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3");
            webclient.Headers.Add("Accept-Encoding", "gzip, deflate");
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            webclient.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Uri uri = new Uri("http://" + ipaddress + "/go_list");
            String data = "--" + leadsloadReq + tabvalselReq + leadfile_nameReq + fileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + postalgmtReq + submit_fileReq + boundary + "--\r\n";
            var datacollection = webclient.Encoding.GetBytes(data);
            var result = webclient.UploadData(uri, "POST", datacollection);



            //SECONDA FASE DEL CARICAMENTO
            boundary = "---------------------------" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            webclient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            //---Settings data Part
            leadsloadReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadsload\"" + lineTerm + ContentTypeBlank + "okfinal" + lineTerm + "--";
            String lead_fileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"lead_file\"" + lineTerm + ContentTypeBlank + "/tmp/admin_listapulita.txt" + lineTerm + "--";
            String leadfileReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile\"" + lineTerm + ContentTypeBlank + "Array" + lineTerm + "--";
            list_id_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"list_id_override\"" + lineTerm + ContentTypeBlank + list_id_overrideVal + lineTerm + "--";
            phone_code_overrideReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_code_override\"" + lineTerm + ContentTypeBlank + phone_code_overrideVal + lineTerm + "--";
            dupcheckReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"dupcheck\"" + lineTerm + ContentTypeBlank + dupcheckVal + lineTerm + "--";
            leadfile_nameReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"leadfile_name\"" + lineTerm + ContentTypeBlank + leadfile_nameVal + lineTerm + "--";
            String superfinalReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"superfinal\"" + lineTerm + ContentTypeBlank + lineTerm + "--";
            String vendor_lead_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"vendor_lead_code_field\"" + lineTerm + ContentTypeBlank + vendor_lead_code_fieldVal + lineTerm + "--";
            String source_id_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"source_id_field\"" + lineTerm + ContentTypeBlank + source_id_fieldVal + lineTerm + "--";
            String phone_number_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"phone_number_field\"" + lineTerm + ContentTypeBlank + phone_number_fieldVal + lineTerm + "--";
            String title_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"title_field\"" + lineTerm + ContentTypeBlank + title_fieldVal + lineTerm + "--";
            String first_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"first_name_field\"" + lineTerm + ContentTypeBlank + first_name_fieldVal + lineTerm + "--";
            String middle_initial_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"middle_initial_field\"" + lineTerm + ContentTypeBlank + middle_initial_fieldVal + lineTerm + "--";
            String last_name_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"last_name_field\"" + lineTerm + ContentTypeBlank + last_name_fieldVal + lineTerm + "--";
            String address1_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address1_field\"" + lineTerm + ContentTypeBlank + address1_fieldVal + lineTerm + "--";
            String address2_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address2_field\"" + lineTerm + ContentTypeBlank + address2_fieldVal + lineTerm + "--";
            String address3_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"address3_field\"" + lineTerm + ContentTypeBlank + address3_fieldVal + lineTerm + "--";
            String city_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"city_field\"" + lineTerm + ContentTypeBlank + city_fieldVal + lineTerm + "--";
            String state_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"state_field\"" + lineTerm + ContentTypeBlank + state_fieldVal + lineTerm + "--";
            String province_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"province_field\"" + lineTerm + ContentTypeBlank + province_fieldVal + lineTerm + "--";
            String postal_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"postal_code_field\"" + lineTerm + ContentTypeBlank + postal_code_fieldVal + lineTerm + "--";
            String country_code_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"country_code_field\"" + lineTerm + ContentTypeBlank + country_code_fieldVal + lineTerm + "--";
            String gender_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"gender_field\"" + lineTerm + ContentTypeBlank + gender_fieldVal + lineTerm + "--";
            String date_of_birth_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"date_of_birth_field\"" + lineTerm + ContentTypeBlank + date_of_birth_fieldVal + lineTerm + "--";
            String alt_phone_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"alt_phone_field\"" + lineTerm + ContentTypeBlank + alt_phone_fieldVal + lineTerm + "--";
            String email_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"email_field\"" + lineTerm + ContentTypeBlank + email_fieldVal + lineTerm + "--";
            String security_phrase_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"security_phrase_field\"" + lineTerm + ContentTypeBlank + security_phrase_fieldVal + lineTerm + "--";
            String comments_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"comments_field\"" + lineTerm + ContentTypeBlank + comments_fieldVal + lineTerm + "--";
            String rank_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"rank_field\"" + lineTerm + ContentTypeBlank + rank_fieldVal + lineTerm + "--";
            String owner_fieldReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"owner_field\"" + lineTerm + ContentTypeBlank + owner_fieldVal + lineTerm + "--";
            String OK_to_processReq = boundary + lineTerm + "Content-Disposition: form-data; name=\"OK_to_process\"" + lineTerm + ContentTypeBlank + "PROCEDI" + lineTerm + "--";
            //Complete Data String
            data = "--" + leadsloadReq + lead_fileReq + leadfileReq + list_id_overrideReq + phone_code_overrideReq + dupcheckReq + leadfile_nameReq + superfinalReq + vendor_lead_code_fieldReq + source_id_fieldReq + phone_number_fieldReq + title_fieldReq + first_name_fieldReq + middle_initial_fieldReq + last_name_fieldReq + address1_fieldReq + address2_fieldReq + address3_fieldReq + city_fieldReq + state_fieldReq + province_fieldReq + postal_code_fieldReq + country_code_fieldReq + gender_fieldReq + date_of_birth_fieldReq + alt_phone_fieldReq + email_fieldReq + security_phrase_fieldReq + comments_fieldReq + rank_fieldReq + owner_fieldReq + OK_to_processReq + boundary + "--\r\n";
            datacollection = webclient.Encoding.GetBytes(data);
            result = webclient.UploadData(uri, "POST", datacollection);
            webclient.SetTimeout(0);
        }
        public void CreaLista(String _list_name, String _list_description, String _campaign_id)
        {
            GetListe();
            //Calculate new ListID
            int listid = Convert.ToInt32(LastListId) + 1;
            LastListId = listid.ToString();
            //Do Things
            String urlreq = "https://" + ipaddress + "/go_list";
            String selectVal = "";
            String addSUBMIT = "addSUBMIT";
            String auto_gen = "on";
            String list_id = LastListId;
            String list_name = _list_name;
            String list_description = _list_description;
            String campaign_id = _campaign_id;
            String active = "Y";

            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "selectval", selectVal },
                { "addSUBMIT", addSUBMIT },
                { "auto_gen", auto_gen },
                { "list_id", list_id },
                { "list_name", list_name },
                { "list_description", list_description },
                { "campaign_id", campaign_id },
                { "active", active }
            };
            Byte[] Response = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list", "POST", reqparm);
        }
        //Get Header From file excel to find number coloumn index
        public void LoadExcelFileHeader(String _filePath)
        {
            //Read Excel File
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(_filePath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            String str;
            int cCnt;
            int rw = 0;
            int cl = 0;
            rw = xlRange.Rows.Count;
            cl = xlRange.Columns.Count;
            for (cCnt = 1; cCnt <= cl; cCnt++)
            {
                str = (String)(xlRange.Cells[1, cCnt] as Excel.Range).Value2;
                int index = cCnt - 1;
                ExcelFirstRow[index] = str;
            }
            //CLEANUP
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad
            //release com objects to fully kill excel process from running in the background
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRange);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorksheet);
            //close and release
            xlWorkbook.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
            //quit and release
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
        }
        public void GetListEsiti(String ListaID)
        {
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "items", "showval=" + ListaID },
                { "action", "editlist" }
            };
            String source = ASCIIEncoding.ASCII.GetString(webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/editview", "POST", reqparm));
            String Esiti = source.Substring(source.IndexOf("##") + 2, (source.Length - (source.IndexOf("##") + 2)));
            source = source.Substring(0, source.IndexOf("##"));
            String[] sourceArray = source.Split(new String[] { "--" }, StringSplitOptions.None);
            //Check if Esiti contains data
            if (String.IsNullOrEmpty(Esiti)) { ListEsitiArr = new String[] { "Nessun esito in questa lista.  List ID:" + ListaID }; return; }
            //Operation on results
            Esiti = Esiti.Substring(0, Esiti.IndexOf("</center>")).Replace("<tr align=", "\n").Replace("<tr class=", "\n");
            Esiti = Esiti.Substring(0, Esiti.IndexOf("<center>"));
            Esiti = Esiti.Replace("left class=tr1><td>", "").Replace("left class=tr2><td>", "");
            Esiti = Esiti.Substring(Esiti.IndexOf("<br>CALLED</td><tr>") + 20, Esiti.Length - (Esiti.IndexOf("<br>CALLED</td><tr>") + 20));
            Esiti = Esiti.Replace("</td></tr>", "").Replace("</td><td>", ";").Replace("</td><td align=\"center\">", ";");
            Esiti = Esiti.Replace("\"tr2\"><td colspan=2><b>", "").Replace("\"tr1\"><td colspan=2 align=left><b>", "");
            Esiti = Esiti.Replace("<b> <font color=\"green\"> ", "").Replace("<b><font color=\"green\">", "");
            Esiti = Esiti.Replace("</td><td colspan=2 align=center><font color=\"blue\"><b>", ";").Replace("</font></table><br><br>", "");
            Esiti = Esiti.Replace("</font>", "").Replace("<b>", "");
            //Finally Make Array with data
            ListEsitiArr = Esiti.Split("\n".ToCharArray());
        }
        public void DownloadList(String ListaID)
        {
            //Choose where to save
            SaveFileDialog saveto = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Testo|*.txt",
                AddExtension = true,
                FileName = "list.txt"
            };
            saveto.ShowDialog();
            //Login old interface
            String authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + userpass));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#################################################################################
            webclient.DownloadFile("https://" + ipaddress + "/_vicidial_/list_download.php?list_id=" + ListaID, saveto.FileName);
        }
        public void CercaNumero(String numero)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "archive_search", "No" },
                { "phone", numero },
                { "submit", "SUBMIT" },
                { "alt_phone_search", "No" }
            };
            String source = ASCIIEncoding.ASCII.GetString(webclient.UploadValues("https://" + ipaddress + "/_vicidial_/admin_search_lead.php", "POST", reqparm));
            //Check if results are present
            if (source.Contains("Please go back and double check the information you entered and submit again"))
            {
                SearchResults[0] = "Nessun Risultato";
                return;
            }
            //Make some operation on output
            int res0 = source.IndexOf("<b>RESULTS: ");
            int res1 = source.IndexOf("</TABLE>", res0);
            source = source.Substring(res0, res1 - res0);
            //some operation
            res0 = source.IndexOf("</TR>");
            source = source.Substring(res0 + 6, source.Length - (res0 + 7));

            source = source.Replace("<TR bgcolor=\"#B9CBFD\">\n", "").Replace("<TR bgcolor=\"#9BB9FB\">\n", "").Replace("<TR bgcolor=\"#B9CBFD\">", "").Replace("<TR bgcolor=\"#9BB9FB\">", "");
            source = source.Substring(0, source.Length - 6);

            source = source.Replace("<TD ALIGN=LEFT><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1>", "").Replace("<TD ALIGN=CENTER><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1><a href=\"admin_modify_lead.php?lead_id=", "").Replace("&archive_search=No\" target=\"_blank\">", ",");
            source = source.Replace("</a></FONT>", "").Replace("<TD ALIGN=CENTER><FONT FACE=\"ARIAL,HELVETICA\" SIZE=1>", "").Replace("</FONT>", "");
            source = source.Replace("</TR>", ";").Replace("</TD>\n", ",").Replace("</TD>", "").Replace("\n", "");
            //write array
            SearchResults = source.Split(";".ToCharArray());
        }
        public void CancellaLista(String _listid)
        {
            webclient.Credentials = new NetworkCredential(username, userpass);
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "listid_delete", _listid },
                { "action", "deletelist" }
            };
            Byte[] responsebytes = webclient.UploadValues("https://" + ipaddress + "/index.php/go_list/deletesubmit", "POST", reqparm);
        }





        //#########################################################################################################################################
        //##############################################           GESTIONE ESITI             #####################################################
        public void MoveLeads(String _listIDFrom, String _ListIDTo, String _moveStatus)
        {
            //Login old interface
            String authInfo = username + ":" + userpass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#######################################################
            String ListIDFrom = _listIDFrom;
            String ListIDTo = _ListIDTo;
            String moveStatus = _moveStatus;
            String moveOp = "<";
            String moveCount = "20";
            //Setting Up web POST Request
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "move_from_list", ListIDFrom },
                { "move_to_list", ListIDTo },
                { "move_status", moveStatus },
                { "move_count_op", moveOp },
                { "move_count_num", moveCount },
                { "confirm_move", "confirm" }
            };
            Byte[] Response = webclient.UploadValues("http://" + ipaddress + "/_vicidial_/lead_tools.php", "POST", reqparm);
            String data = ASCIIEncoding.ASCII.GetString(Response);
        }
        public void UpdateLeads(String _listID, String _fromStatus, String _toStatus)
        {
            //Login old interface
            String authInfo = username + ":" + userpass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webclient.Headers["Authorization"] = "Basic " + authInfo;
            //#######################################################
            String ListID = _listID;
            String fromStatus = _fromStatus;
            String toStatus = _toStatus;
            String moveOp = "<";
            String moveCount = "20";
            //Setting Up web POST Request
            var reqparm = new System.Collections.Specialized.NameValueCollection
            {
                { "update_list", ListID },
                { "update_from_status", fromStatus },
                { "update_to_status", toStatus },
                { "update_count_op", moveOp },
                { "update_count_num", moveCount },
                { "confirm_update", "confirm" }
            };
            Byte[] Response = webclient.UploadValues("http://" + ipaddress + "/_vicidial_/lead_tools.php", "POST", reqparm);
            String data = ASCIIEncoding.ASCII.GetString(Response);
        }






        //Get Server Status
        public void GetServerInfo()
        {
            //Get Vitals Data
            String source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/vitals.php").Replace("\t", "");
            String[] SourceArray = source.Split("\n".ToCharArray());
            int host0 = SourceArray[8].IndexOf("<td class=\"tabdata\">");
            int host1 = SourceArray[8].IndexOf("</font></td>", host0);
            Hostname = SourceArray[8].Substring(host0 + 20, host1 - (host0 + 20));
            //Listening IP
            int listip0 = SourceArray[14].IndexOf("<td class=\"tabdata\">");
            int listip1 = SourceArray[14].IndexOf("</font></td>", listip0);
            ListeningIP = SourceArray[14].Substring(listip0 + 20, listip1 - (listip0 + 20));
            //Kernel Version
            int kerver0 = SourceArray[20].IndexOf("<td class=\"tabdata\" >");
            int kerver1 = SourceArray[20].IndexOf("</font></td>", kerver0);
            KernelVersion = SourceArray[20].Substring(kerver0 + 21, kerver1 - (kerver0 + 21));
            //Distro Name
            int dname0 = SourceArray[26].IndexOf("&nbsp;");
            DistroName = SourceArray[26].Substring(dname0 + 6, SourceArray[26].Length - (dname0 + 6));
            //Uptime
            int upt0 = SourceArray[33].IndexOf("<td class=\"tabdata\">");
            int upt1 = SourceArray[33].IndexOf("</font></td>", upt0);
            Uptime = SourceArray[33].Substring(upt0 + 20, upt1 - (upt0 + 20)).Replace("&nbsp;", " ");
            //Load Averages
            int loada0 = SourceArray[39].IndexOf("&nbsp;");
            int loada1 = SourceArray[39].IndexOf("</font></td>", loada0);
            LoadAverages = SourceArray[39].Substring(loada0 + 6, loada1 - (loada0 + 6));
            //Used Physical Memory (RAM)
            source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/memory.php").Replace("\t", "");
            SourceArray = source.Split("\n".ToCharArray());
            int pym0 = SourceArray[25].IndexOf("&nbsp;&nbsp;");
            int pym1 = SourceArray[25].IndexOf("</font></td>", pym0);
            PhysicalMemoryUsed = SourceArray[25].Substring(pym0 + 12, pym1 - (pym0 + 12));
            //Total Disk Usage
            source = webclient.DownloadString("https://" + ipaddress + "/application/views/phpsysinfo/filesystems.php").Replace(" ", "");
            SourceArray = source.Split("\n".ToCharArray());
            int totdisk0 = SourceArray[59].IndexOf("&nbsp;");
            int totdisk1 = SourceArray[59].IndexOf("</font></td>", totdisk0);
            TotalDiskUsage = SourceArray[59].Substring(totdisk0 + 6, totdisk1 - (totdisk0 + 6));
        }
    }
}
