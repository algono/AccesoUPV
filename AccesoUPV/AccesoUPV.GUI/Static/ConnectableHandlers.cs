using AccesoUPV.GUI.UserControls;
using AccesoUPV.GUI.Windows;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Interfaces;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace AccesoUPV.GUI.Static
{
    public static class ConnectableHandlers
    {
        public static async Task ConnectWDrive(IAccesoUPVService service, ConnectionEventArgs e)
        {
            try
            {
                NetworkDrive drive = e.Connectable as NetworkDrive;
                if (drive.NeedsPassword)
                {
                    do
                    {
                        if (string.IsNullOrEmpty(drive.Password))
                        {
                            drive.Password = Interaction.InputBox("Introduzca la contraseña de la UPV:", "Conectar Disco W");
                        }
                        try
                        {
                            await ConnectDrive(service, e, false);
                        }
                        catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(drive.Password)))
                        {
                            MessageBox.Show("No ha indicado ninguna contraseña.", "Falta contraseña",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            drive.Password = null;
                        }
                        catch (ArgumentException ex) when (ex.ParamName.Equals(nameof(drive.Password)))
                        {
                            MessageBox.Show("Contraseña incorrecta.", "Contraseña incorrecta",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            drive.Password = null;
                        }
                    } while (drive.Password == null);
                }
                else
                {
                    await ConnectDrive(service, e);
                }
            }
            catch (CredentialsBugException ex)
            {
                bool retry = await HandleCrendentialsError(service, ex);
                if (retry) await ConnectWDrive(service, e);
            }
        }

        /// <summary>
        /// Tries to handle the CredentialsBugException, and returns if the connection process should be retried.
        /// </summary>
        /// <returns>Retry: If the connection process should be retried.</returns>
        private static async Task<bool> HandleCrendentialsError(IAccesoUPVService service, CredentialsBugException ex)
        {
            try
            {
                bool didAnything = await service.ResetUPVConnectionAsync();

                if (!didAnything)
                {
                    MessageBox.Show(ex.Message, "Error credenciales", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return didAnything;
            }
            catch (OperationCanceledException cancelledException)
            {
                const string CanceledReconnectionMessage =
                    "Cuidado, si cancela la operación de reconexión, el programa podría dejar de funcionar correctamente.\n\n" +
                    "¿Seguro que desea continuar?";

                bool cancelationHandled = false;
                while (!cancelationHandled)
                {
                    MessageBoxResult result = MessageBox.Show(CanceledReconnectionMessage, "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        throw cancelledException;
                    }
                    else
                    {
                        try
                        {
                            await service.VPN_UPV.ConnectAsync();
                            cancelationHandled = true;
                        }
                        // The cancelation message will be shown until the VPN is connected or the user says "Yes"
                        catch (OperationCanceledException) { }
                    }
                }
            }
            catch (TimeoutException tex)
            {
                MessageBox.Show(tex.Message, "El tiempo de espera ha expirado", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        public static Task ConnectDrive(IAccesoUPVService service, ConnectionEventArgs e)
            => ConnectDrive(service, e, true);
        private static async Task ConnectDrive(IAccesoUPVService service, ConnectionEventArgs e, bool catchPasswordErrors)
        {
            NetworkDrive networkDrive = e.Connectable as NetworkDrive;
            try
            {
                await e.ConnectionFunc();
            }
            catch (NotAvailableDriveException ex) when (ex.Letter == default)
            {
                MessageBox.Show(ex.Message, "Ninguna unidad disponible", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotAvailableDriveException ex)
            {
                string WARNING_W =
                    ex.Message + "\n" +
                    "(Continúe si prefiere que se elija la primera unidad disponible solo durante esta conexión).\n ";

                MessageBoxResult result = MessageBox.Show(WARNING_W, $"Unidad {ex.Letter} contiene disco",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    networkDrive.Letter = default;
                    await e.ConnectionFunc().ContinueWith((t) => networkDrive.Letter = ex.Letter);
                }
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(networkDrive.Username)))
            {
                MessageBox.Show("No ha indicado ningún nombre de usuario. Indique uno en los ajustes.", "Falta usuario",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                OpenPreferences(service);
            }
            catch (ArgumentNullException ex) when (catchPasswordErrors && ex.ParamName.Equals(nameof(networkDrive.Password)))
            {
                MessageBox.Show("No ha indicado ninguna contraseña. Indíquela en los ajustes.", "Falta contraseña",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                OpenPreferences(service);
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName.Equals(nameof(networkDrive.Address)))
            {
                /**
                 * We dont ask the user for the address, but the address depends on the username
                 * (which is, in fact, asked to the user). So we assume the error is because of the username.
                */
                MessageBox.Show("Nombre de usuario incorrecto.", "Usuario incorrecto",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                OpenPreferences(service);
            }
            catch (ArgumentException ex) when (catchPasswordErrors && ex.ParamName.Equals(nameof(networkDrive.Password)))
            {
                MessageBox.Show("Contraseña incorrecta.", "Contraseña incorrecta",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                OpenPreferences(service);
            }
        }

        public static async Task DisconnectDrive(ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (OpenedFilesException ex)
            {
                await Drive_OpenedFiles(ex);
            }
        }

        private static async Task Drive_OpenedFiles(OpenedFilesException ex)
        {
            MessageBoxResult result = MessageBox.Show(ex.Message, OpenedFilesException.WarningTitle,
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                await ex.ContinueAsync();
            }
        }

        public static async Task ConnectPortalDSIC(IAccesoUPVService service, ConnectionEventArgs e)
        {
            try
            {
                #region Conflicto con Disco W
                if (service.Disco_W.IsConnected)
                {
                    MessageBoxResult result = MessageBox.Show("No se puede acceder a la VPN del DSIC teniendo el Disco W conectado.\n"
                                    + "Si continúa, este será desconectado automáticamente.\n\n"
                                    + "¿Desea continuar?", "Conflicto entre conexiones", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        await service.Disco_W.DisconnectAsync();
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }
                #endregion

                await e.ConnectionFunc();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No ha indicado ningún nombre para la VPN del DSIC. Indique uno en los ajustes.",
                    "Falta nombre",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                OpenPreferences(service);
            }

        }

        public static void OpenPreferences(IAccesoUPVService service)
        {
            new Preferences(service).ShowDialog();
        }

        public static void ConnectToSSH(IAccesoUPVService service, string server)
        {
            try
            {
                if (string.IsNullOrEmpty(service.User)) throw new ArgumentNullException();
                SSHConnection.ConnectTo(server, service.User);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No ha indicado ningún nombre de usuario. Indique uno en los ajustes.", "Falta usuario",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                OpenPreferences(service);
            }
        }


        #region IsConnected Bindings
        private static readonly Dictionary<DependencyObject, List<Tuple<IConnectable, EventHandler>>> _connectionStatusHandlerDictionary
          = new Dictionary<DependencyObject, List<Tuple<IConnectable, EventHandler>>>();

        public static void Bind(DependencyObject d, IConnectable connectable, EventHandler OnConnectionStatusChanged)
        {
            Bind(connectable, OnConnectionStatusChanged);

            _connectionStatusHandlerDictionary.TryGetValue(d, out List<Tuple<IConnectable, EventHandler>> bindings);

            if (bindings == null)
            {
                bindings = new List<Tuple<IConnectable, EventHandler>>();
                _connectionStatusHandlerDictionary.Add(d, bindings);
            }

            bindings.Add(new Tuple<IConnectable, EventHandler>(connectable, OnConnectionStatusChanged));
        }

        private static void Bind(IConnectable connectable, EventHandler OnConnectionStatusChanged)
        {
            connectable.Connected += OnConnectionStatusChanged;
            connectable.Disconnected += OnConnectionStatusChanged;
        }

        public static void Unbind(DependencyObject d, IConnectable connectable)
        {
            _connectionStatusHandlerDictionary.TryGetValue(d, out List<Tuple<IConnectable, EventHandler>> bindings);

            if (bindings != null)
            {
                List<int> bindingIndexesToRemove = new List<int>();

                for (int i = 0; i < bindings.Count; i++)
                {
                    Tuple<IConnectable, EventHandler> binding = bindings[i];
                    if (binding.Item1 == connectable)
                    {
                        EventHandler OnConnectionStatusChanged = binding.Item2;
                        Unbind(connectable, OnConnectionStatusChanged);

                        bindingIndexesToRemove.Add(i);
                    }
                }

                bindingIndexesToRemove.ForEach(index => bindings.RemoveAt(index));
            }
        }

        private static void Unbind(IConnectable connectable, EventHandler OnConnectionStatusChanged)
        {
            connectable.Connected -= OnConnectionStatusChanged;
            connectable.Disconnected -= OnConnectionStatusChanged;
        }

        public static EventHandler CreateOnConnectionStatusChanged(DependencyObject d, DependencyProperty dp, bool inverted = false)
            => delegate (object sender, EventArgs e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IConnectable connectable = ((IConnectable)sender);
                    bool isConnected = connectable?.IsConnected ?? false;
                    if (inverted) isConnected = !isConnected;
                    d.SetValue(dp, isConnected);
                });
            };
    } 
    #endregion
}
