/*eslint no-console: ["error", { allow: ["log", warn", "error"] }] */
import Ember from 'ember';

const {
  inject,
  A,
} = Ember;

const {service} = inject;

export default Ember.Controller.extend({
  searchUrl: null,

  requestStarted: false,
  displayResults: false,
  expandLogs: true,
  results: null,

  messages: A(),

  stream: service(),

  init() {
    this.get('stream').connect(payload => {
      this._handleMessage(JSON.parse(payload));
    });
    this._super(...arguments);
  },

  _handleMessage(payload) {
    let messages = this.get('messages');
    if (typeof payload.message === 'string') {
      let payloadMessages = payload.message.split('\n');
      payloadMessages.forEach(m => {
        messages.pushObject(m);
      });
    }

    if (payload.endOfChain === true) {
      this.set('loading', false);
      if (payload.completedSuccessfully === true) {
        this.set('expandLogs', false);
        this.set('displayResults', true);
        this.set('results', payload.message);
      }
    }
  },

  actions: {
    search(url) {
      this.set('requestStarted', true);
      this._request(url);
      this.set('loading', true);
    },
  },

  _request(url) {
    this.set('displayResults', false);
    this.get('stream').send(url);
  },

  _displayResults(result) {
    this.set('displayResults', true);
    this.set('results', result);
  },

  _displayMessage(msg) {
    this.set('displayResults', false);
    this.set('message', msg);
  },
});
