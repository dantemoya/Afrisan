using Afrisan.Nuevo.Api;
using Afrisan.Nuevo.Contratos;
using System.Security.Claims;
using System.Security.Cryptography;
var inv = new InventarioStore(); var sesiones = new SesionesDemo(RandomNumberGenerator.GetBytes(64)); var trabajo = new TrabajoStore(inv, sesiones);
ClaimsPrincipal User(string role) => new(new ClaimsIdentity(new[]{new Claim(ClaimTypes.NameIdentifier, role.ToLowerInvariant()+"@afrisan.example"),new Claim(ClaimTypes.Role,role)},"Prueba"));
void Check(bool ok,string msg) { if(!ok) throw new Exception(msg); Console.WriteLine("OK: "+msg); }
var gas=inv.CrearRefrigerante(new(){Codigo="R-TEST",Nombre="Solo regresión"});
CrearCilindroRequest Base(string ubicacion,string? sucursal=null) => new(){RefrigeranteId=gas.Id,TaraGr=1000,CapacidadGr=10000,PesoInicialGr=7000,UbicacionInicial=ubicacion,SucursalInicial=sucursal};
var viejo=inv.CrearCilindro(Base("Sucursal","Centro"),"prueba"); var nuevo=inv.CrearCilindro(Base("Bodega"),"prueba");
Check(inv.Cilindros().Count(c=>c.Ubicacion=="Bodega" && c.Estado=="Disponible")==1,"El instalado no suma disponibles de Bodega");
CrearOtRequest Ot(string codigo,string sucursal="Centro")=>new(){IdOperacion=Guid.NewGuid(),Sucursal=sucursal,Direccion="Prueba",Descripcion="Reemplazo",TecnicoEmail="tecnico@afrisan.example",Detalle=[new(){RefrigeranteId=gas.Id,GrSolicitados=6000,GrAutorizados=6000}],CilindrosARetirar=[codigo]};
try { trabajo.Crear(Ot(nuevo.Codigo),User("Autorizador"));throw new Exception("Aceptó retiro desde Bodega"); } catch(ReglaInventarioException e) {Check(e.Estado==409,"Rechaza usar Bodega como retiro de sucursal");}
try { trabajo.Crear(Ot(viejo.Codigo,"Otra"),User("Autorizador"));throw new Exception("Aceptó otra sucursal"); } catch(ReglaInventarioException e) {Check(e.Estado==409,"Rechaza retirar de otra sucursal");}
var ot=trabajo.Crear(Ot(viejo.Codigo),User("Autorizador"));
Check(inv.Cilindro(nuevo.Codigo)!.Ubicacion=="Bodega","Crear OT no mueve el reemplazo de Bodega");
trabajo.Entregar(ot.Id,new(){Codigo=nuevo.Codigo},User("Bodega"));
Check(inv.Cilindro(nuevo.Codigo)!.Ubicacion=="Tecnico","La entrega sí toma el cilindro de Bodega");
var op=trabajo.Guardar(ot.Id,new(){Id=Guid.NewGuid(),Instancia=sesiones.Instancia,Tipo="Reemplazo",CodigoRetirado=viejo.Codigo,CodigoInstalado=nuevo.Codigo,PesoRetiradoGr=2000,PesoInstaladoGr=7000},User("Tecnico"));
// Imágenes sintéticas exclusivas de la regresión, nunca registros del cliente.
foreach(var grupo in new[]{"Retirado","Instalado"}) {
 var bytes=File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory,grupo+".png"));
 trabajo.Foto(ot.Id,op.Id,new(){Id=Guid.NewGuid(),Grupo=grupo,Nombre=grupo+".png",ContentType="image/png",Base64=Convert.ToBase64String(bytes)},User("Tecnico"));
}
trabajo.Confirmar(ot.Id,op.Id,User("Tecnico"));
Check(inv.Cilindro(nuevo.Codigo)!.Ubicacion=="Sucursal","El entregado queda instalado en sucursal");
Check(inv.Cilindro(viejo.Codigo)!.Ubicacion=="Tecnico","El retirado aún no cuenta como recibido en Bodega");
trabajo.Recibir(ot.Id,new(){Codigo=viejo.Codigo,PesoBrutoGr=2000},User("Bodega"));
Check(inv.Cilindro(viejo.Codigo)!.Ubicacion=="Bodega","Solo la recepción suma el retirado a Bodega");
trabajo.Terminar(ot.Id,new(),User("Tecnico"));trabajo.Cerrar(ot.Id,User("Autorizador"));
Check(trabajo.Obtener(ot.Id,User("Autorizador")).Estado=="Cerrada","La OT de reemplazo puede cerrarse");
