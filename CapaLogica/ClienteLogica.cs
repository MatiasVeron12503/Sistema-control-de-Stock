using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaMayorista.CapaDatos;
using VentaMayorista.CapaEntidades;

namespace VentaMayorista.CapaLogica
{
    public class ClienteLogica
    {
        private readonly ClienteDatos clienteDatos = new ClienteDatos();

        public void Crear(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(cliente.Nombre)) throw new Exception("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(cliente.Apellido)) throw new Exception("El apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(cliente.DNI)) throw new Exception("El DNI es obligatorio");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cliente.DNI, @"^\d{8}$"))
                throw new Exception("El DNI debe contener exactamente 8 dígitos");

            // Validar email si se proporciona
            if (!string.IsNullOrWhiteSpace(cliente.Email) &&
                !System.Text.RegularExpressions.Regex.IsMatch(cliente.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del email es inválido");

            clienteDatos.Crear(cliente);
        }

        public List<Cliente> ObtenerTodos()
        {
            if (clienteDatos == null)
                return new List<Cliente>();
            try
            {
                return clienteDatos.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener clientes: {ex.Message}", ex);
            }
        }

        public Cliente ObtenerPorId(int idCliente)
        {
            if (idCliente <= 0) throw new ArgumentException("El ID del cliente debe ser mayor a 0");

            var cliente = clienteDatos.ObtenerPorId(idCliente);
            if (cliente == null) throw new Exception("El cliente no existe");

            return cliente;
        }

        public void Actualizar(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (cliente.IdCliente <= 0) throw new Exception("El ID del cliente es inválido");
            if (string.IsNullOrWhiteSpace(cliente.Nombre)) throw new Exception("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(cliente.Apellido)) throw new Exception("El apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(cliente.DNI)) throw new Exception("El DNI es obligatorio");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cliente.DNI, @"^\d{8}$"))
                throw new Exception("El DNI debe contener exactamente 8 dígitos");

            // Validar email si se proporciona
            if (!string.IsNullOrWhiteSpace(cliente.Email) &&
                !System.Text.RegularExpressions.Regex.IsMatch(cliente.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del email es inválido");

            // Verificar que el cliente exista
            var clienteExistente = clienteDatos.ObtenerPorId(cliente.IdCliente);
            if (clienteExistente == null) throw new Exception("El cliente no existe");

            clienteDatos.Actualizar(cliente);
        }

        public void Eliminar(int idCliente)
        {
            if (idCliente <= 0) throw new ArgumentException("El ID del cliente debe ser mayor a 0");

            // Verificar que el cliente exista
            var cliente = clienteDatos.ObtenerPorId(idCliente);
            if (cliente == null) throw new Exception("El cliente no existe");

            clienteDatos.Eliminar(idCliente);
        }
    }
}
