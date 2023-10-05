using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using dom;



namespace negcio
{
   public class ControlNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select  Codigo, A.Id, Nombre, A.Descripcion, ImagenUrl, IdMarca, IdCategoria, C.Descripcion as Categoria, M.Descripcion as Marca, Precio From ARTICULOS A, CATEGORIAS C, MARCAS M where C.id = IdCategoria AND M.id = idMarca";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();


                while (lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.codigo = (string)lector["codigo"];
                    aux.nombre = (string)lector["Nombre"];
                    aux.id = (int)lector["id"];
                    aux.descripcion = (string)lector["Descripcion"];

                    if (!(lector.IsDBNull(lector.GetOrdinal("ImagenUrl")))) //Ejemplo Si es null, aunque imagenUrl acepta null en la BD.
                    aux.imagenUrl = (string)lector["ImagenUrl"];

                    aux.marca = new Marca();
                    aux.marca.Id = (int)lector["idMarca"];
                    aux.marca.descripcion = (string)lector["marca"];
                    aux.categoria = new Categoria();
                    aux.categoria.Id = (int)lector["idcategoria"];
                    aux.categoria.descripcion = (string)lector["categoria"];
                    aux.precio = (decimal)lector["precio"];

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void agregar(Articulo nuevo)
        {
            AccesoAdatos datos = new AccesoAdatos();
            try
            {
                datos.SetearConsulta("insert into ARTICULOS  (Codigo,Nombre,Descripcion,ImagenUrl,Precio,idMarca,idCategoria) values ('"+nuevo.codigo+"','"+nuevo.nombre+"','"+nuevo.descripcion+"','"+nuevo.imagenUrl+"','"+nuevo.precio+"',@IdMarca,@IdCategoria)"); 
                datos.SetearParametros("@IdMarca",nuevo.marca.Id);
                datos.SetearParametros("@IdCategoria", nuevo.categoria.Id);
              
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
        public void modificar (Articulo modificar)
        {
            AccesoAdatos datos = new AccesoAdatos();
            try
            {
                datos.SetearConsulta("update ARTICULOS set Nombre = @Nombre, Codigo = @Codigo, Descripcion = @Descripcion, ImagenUrl = @ImagenUrl, IdCategoria = @IdCategoria, IdMarca = @IdMarca, Precio = @Precio where id = @id");
                datos.SetearParametros("@Nombre", modificar.nombre);
                datos.SetearParametros("@Codigo", modificar.codigo);
                datos.SetearParametros("@Descripcion", modificar.descripcion);
                datos.SetearParametros("@ImagenUrl", modificar.imagenUrl);
                datos.SetearParametros("@IdCategoria", modificar.categoria.Id);
                datos.SetearParametros("@IdMarca", modificar.marca.Id);
                datos.SetearParametros("@id", modificar.id);
                datos.SetearParametros("@Precio", modificar.precio);

                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
            
        }
        public void eliminar(int Id)
        {   
            try
            {
                AccesoAdatos datos = new AccesoAdatos();
                datos.SetearConsulta("Delete From ARTICULOS where id = @id");
                datos.SetearParametros("@id",Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List <Articulo> lista = new List<Articulo>();
            AccesoAdatos datos = new AccesoAdatos();
            try
            {

                string consulta = "Select  Codigo, A.Id, Nombre, A.Descripcion, ImagenUrl, IdMarca, IdCategoria, C.Descripcion as Categoria, M.Descripcion as Marca, Precio From ARTICULOS A, CATEGORIAS C, MARCAS M where C.id = IdCategoria AND M.id = idMarca AND ";
                

                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;
                            break;
                        default:
                            consulta += "Precio = " + filtro;
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "M.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "M.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "M.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "C.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "C.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "C.Descripcion like '%" + filtro + "%'";
                            break;

                    }
                    
                }
                
                datos.SetearConsulta(consulta);
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.codigo = (string)datos.Lector["Codigo"];
                    aux.id = (int)datos.Lector["Id"];
                    aux.nombre = (string)datos.Lector["Nombre"];
                    aux.descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector.IsDBNull(datos.Lector.GetOrdinal("ImagenUrl"))))
                        aux.imagenUrl = (string)datos.Lector["ImagenUrl"];

                    aux.categoria = new Categoria();
                    aux.categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.categoria.descripcion = (string)datos.Lector["Categoria"];
                    aux.marca = new Marca();
                    aux.marca.Id = (int)datos.Lector["IdMarca"];
                    aux.marca.descripcion = (string)datos.Lector["Marca"];
                    aux.precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    
   
}