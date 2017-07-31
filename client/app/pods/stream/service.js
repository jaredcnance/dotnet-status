import Ember from 'ember';
import signalR from 'npm:signalr-client';
import ENV from 'client/config/environment';

export default Ember.Service.extend({
  _connection: null,

  connect(onmessage) {
    let transportType = signalR.TransportType[
      this._getParameterByName('transport')
    ] || signalR.TransportType.WebSockets;

    let connection = new signalR.HttpConnection(ENV.APP.SOCKETS_HOST, {
      transport: transportType,
    });

    connection.onDataReceived = onmessage;

    connection.start();

    this.set('_connection', connection);

    this._super(...arguments);
  },

  send(message) {
    let connection = this.get('_connection');
      connection.send(message);
  },

  _getParameterByName(name, url) {
    if (!url) {
      url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
      results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
  },
});
