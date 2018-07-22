using Paper.Media;
using Paper.Media.Design;
using Paper.Media.Design.Queries;
using Paper.Media.Design.Sql;
using Paper.Media.Design.Widgets;
using Paper.Media.Design.Widgets.Mapping;
using Paper.Media.Rendering.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Toolset.Reflection;

namespace Paper.WebApp.Server.Demo.Consultas
{
  public static class Lacunas
  {
    public class Row
    {
      [DisplayName("Série")]
      public int? Serie { get; set; }

      [DisplayName("Número")]
      public int? Numero { get; set; }

      [DisplayName("Chave da Nota")]
      public string Chave { get; set; }

      [DisplayName("Tipo de Emissão")]
      public string TipoEmissao { get; set; }

      [DisplayName("Situaçãox")]
      public string Situacao { get; set; }

      [DisplayName("Rejeição")]
      public string Rejeicao { get; set; }
    }

    public static List<Row> Rows = new List<Row>();

    private static object[][] data = new object[][]
    {
      new object[] { 12, 57663, "33170206988739000130650120000576631000030017", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57664, "33170206988739000130650120000576641000030022", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57665, "33170206988739000130650120000576651000030038", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57666, "33170206988739000130650120000576661000030043", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57667, "33170206988739000130650120000576671000030059", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57668, "33170206988739000130650120000576681000030064", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57669, "33170206988739000130650120000576691000030070", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57670, "33170206988739000130650120000576701000030089", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57671, "33170206988739000130650120000576711000030094", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57672, "33170206988739000130650120000576721000030105", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57673, "33170206988739000130650120000576731000030110", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57674, "33170206988739000130650120000576741000030126", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57675, "33170206988739000130650120000576751000030131", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57676, "33170206988739000130650120000576761000030147", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57677, "33170206988739000130650120000576771000030152", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57678, "33170206988739000130650120000576781000030168", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57679, "33170206988739000130650120000576791000030173", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57680, "33170206988739000130650120000576801000030182", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57681, "33170206988739000130650120000576811000030198", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57682, "33170206988739000130650120000576821000030209", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57683, "33170206988739000130650120000576831000030214", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57684, "33170206988739000130650120000576841000030220", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57685, "33170206988739000130650120000576851000030235", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57686, "33170206988739000130650120000576861000030240", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57687, "33170206988739000130650120000576871000030256", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57688, "33170206988739000130650120000576881000030261", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57689, "33170206988739000130650120000576891000030277", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57690, "33170206988739000130650120000576901000030286", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57691, "33170206988739000130650120000576911000030291", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57692, "33170206988739000130650120000576921000030302", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57693, "33170206988739000130650120000576931000030318", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57694, "33170206988739000130650120000576941000030323", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57695, "33170206988739000130650120000576951000030339", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57696, "33170206988739000130650120000576961000030344", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57697, "33170206988739000130650120000576971000030350", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57698, "33170206988739000130650120000576981000030365", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57699, "33170206988739000130650120000576991000030370", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57700, "33170206988739000130650120000577001000030381", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57701, "33170206988739000130650120000577011000030397", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57702, "33170206988739000130650120000577021000030408", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57703, "33170206988739000130650120000577031000030413", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57704, "33170206988739000130650120000577041000030429", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57705, "33170206988739000130650120000577051000030434", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57706, "33170206988739000130650120000577061000030440", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57707, "33170206988739000130650120000577071000030455", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57708, "33170206988739000130650120000577081000030460", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57709, "33170206988739000130650120000577091000030476", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57710, "33170206988739000130650120000577101000030485", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57711, "33170206988739000130650120000577111000030490", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57712, "33170206988739000130650120000577121000030501", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57713, "33170206988739000130650120000577131000030517", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57714, "33170206988739000130650120000577141000030522", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57715, "33170206988739000130650120000577151000030538", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57716, "33170206988739000130650120000577161000030543", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57717, "33170206988739000130650120000577171000030559", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57718, "33170206988739000130650120000577181000030564", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57719, "33170206988739000130650120000577191000030570", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57720, "33170206988739000130650120000577201000030589", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57721, "33170206988739000130650120000577211000030594", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57722, "33170206988739000130650120000577221000030605", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57723, "33170206988739000130650120000577231000030610", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57724, "33170206988739000130650120000577241000030626", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57725, "33170206988739000130650120000577251000030631", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57726, "33170206988739000130650120000577261000030647", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57727, "33170206988739000130650120000577271000030652", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57728, "33170206988739000130650120000577281000030668", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57729, "33170206988739000130650120000577291000030673", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57730, "33170206988739000130650120000577301000030682", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57731, "33170206988739000130650120000577311000030698", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57732, "33170206988739000130650120000577321000030709", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57733, "33170206988739000130650120000577331000030714", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57734, "33170206988739000130650120000577341000030720", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57735, "33170206988739000130650120000577351000030735", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57736, "33170206988739000130650120000577361000030740", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57737, "33170206988739000130650120000577371000030756", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57738, "33170206988739000130650120000577381000030761", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57739, "33170206988739000130650120000577391000030777", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57740, "33170206988739000130650120000577401000030786", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57741, "33170206988739000130650120000577411000030791", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57742, "33170206988739000130650120000577421000030802", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57743, "33170206988739000130650120000577431000030818", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57744, "33170206988739000130650120000577441000030823", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57745, "33170206988739000130650120000577451000030839", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57746, "33170206988739000130650120000577461000030844", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57747, "33170206988739000130650120000577471000030850", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57748, "33170206988739000130650120000577481000030865", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57749, "33170206988739000130650120000577491000030870", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57750, "33170206988739000130650120000577501000030880", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57751, "33170206988739000130650120000577511000030895", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57752, "33170206988739000130650120000577521000030906", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57753, "33170206988739000130650120000577531000030911", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57754, "33170206988739000130650120000577541000030927", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57755, "33170206988739000130650120000577551000030932", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57756, "33170206988739000130650120000577561000030948", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57757, "33170206988739000130650120000577571000030953", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57758, "33170206988739000130650120000577581000030969", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57759, "33170206988739000130650120000577591000030974", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57760, "33170206988739000130650120000577601000030983", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57761, "33170206988739000130650120000577611000030999", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57762, "33170206988739000130650120000577621000031003", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57763, "33170206988739000130650120000577631000031019", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57764, "33170206988739000130650120000577641000031024", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57765, "33170206988739000130650120000577651000031030", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57766, "33170206988739000130650120000577661000031045", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57767, "33170206988739000130650120000577671000031050", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57768, "33170206988739000130650120000577681000031066", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57769, "33170206988739000130650120000577691000031071", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57770, "33170206988739000130650120000577701000031080", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57771, "33170206988739000130650120000577711000031096", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57772, "33170206988739000130650120000577721000031107", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57773, "33170206988739000130650120000577731000031112", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57774, "33170206988739000130650120000577741000031128", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57775, "33170206988739000130650120000577751000031133", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57776, "33170206988739000130650120000577761000031149", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57777, "33170206988739000130650120000577771000031154", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57778, "33170206988739000130650120000577781000031160", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57779, "33170206988739000130650120000577791000031175", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57780, "33170206988739000130650120000577801000031184", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57781, "33170206988739000130650120000577811000031190", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57782, "33170206988739000130650120000577821000031200", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57783, "33170206988739000130650120000577831000031216", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57784, "33170206988739000130650120000577841000031221", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57785, "33170206988739000130650120000577851000031237", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57786, "33170206988739000130650120000577861000031242", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57788, "33170206988739000130650120000577881000031263", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57789, "33170206988739000130650120000577891000031279", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57790, "33170206988739000130650120000577901000031288", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57791, "33170206988739000130650120000577911000031293", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57792, "33170206988739000130650120000577921000031304", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57793, "33170206988739000130650120000577931000031310", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57794, "33170206988739000130650120000577941000031325", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57795, "33170206988739000130650120000577951000031330", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57796, "33170206988739000130650120000577961000031346", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57797, "33170206988739000130650120000577971000031351", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57798, "33170206988739000130650120000577981000031367", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57799, "33170206988739000130650120000577991000031372", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57800, "33170206988739000130650120000578001000031383", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57801, "33170206988739000130650120000578011000031399", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57802, "33170206988739000130650120000578021000031400", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57803, "33170206988739000130650120000578031000031415", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57804, "33170206988739000130650120000578041000031420", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57805, "33170206988739000130650120000578051000031436", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57806, "33170206988739000130650120000578061000031441", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57807, "33170206988739000130650120000578071000031457", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57808, "33170206988739000130650120000578081000031462", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57809, "33170206988739000130650120000578091000031478", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57810, "33170206988739000130650120000578101000031487", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57811, "33170206988739000130650120000578111000031492", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57812, "33170206988739000130650120000578121000031503", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57813, "33170206988739000130650120000578131000031519", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57814, "33170206988739000130650120000578141000031524", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57815, "33170206988739000130650120000578151000031530", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57816, "33170206988739000130650120000578161000031545", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57817, "33170206988739000130650120000578171000031550", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57818, "33170206988739000130650120000578181000031566", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57819, "33170206988739000130650120000578191000031571", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57820, "33170206988739000130650120000578201000031580", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57821, "33170206988739000130650120000578211000031596", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57822, "33170206988739000130650120000578221000031607", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57823, "33170206988739000130650120000578231000031612", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57824, "33170206988739000130650120000578241000031628", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57825, "33170206988739000130650120000578251000031633", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57826, "33170206988739000130650120000578261000031649", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57827, "33170206988739000130650120000578271000031654", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57828, "33170206988739000130650120000578281000031660", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57829, "33170206988739000130650120000578291000031675", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57830, "33170206988739000130650120000578301000031684", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57831, "33170206988739000130650120000578311000031690", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57832, "33170206988739000130650120000578321000031700", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57833, "33170206988739000130650120000578331000031716", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57834, "33170206988739000130650120000578341000031721", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57835, "33170206988739000130650120000578351000031737", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57836, "33170206988739000130650120000578361000031742", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57837, "33170206988739000130650120000578371000031758", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57838, "33170206988739000130650120000578381000031763", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57839, "33170206988739000130650120000578391000031779", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57840, "33170206988739000130650120000578401000031788", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57841, "33170206988739000130650120000578411000031793", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57842, "33170206988739000130650120000578421000031804", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57843, "33170206988739000130650120000578431000031810", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." },
      new object[] { 12, 57844, "33170206988739000130650120000578441000031825", "Normal", "Rejeitado", "Rejeição (225): Falha no Schema XML do lote de NFe." }
    };

    static Lacunas()
    {
      foreach (var item in data)
      {
        Rows.Add(new Row
        {
          Serie = (int?)item[0],
          Numero = (int?)item[1],
          Chave = (string)item[2],
          TipoEmissao = (string)item[3],
          Situacao = (string)item[4],
          Rejeicao = (string)item[5],
        });
      }
    }
  }
}