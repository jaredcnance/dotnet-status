import Ember from 'ember';
import ENV from 'client/config/environment';

const {$} = Ember;

export default Ember.Controller.extend({
  searchUrl: null,

  displayResults: false,
  results: null,

  displayMessage: false,
  message: null,

  actions: {
    search(url) {
      let hostUrl = `${ENV.APP.API_HOST}/${url}/`;
      this.set('searchUrl', hostUrl);
      this._request(hostUrl);
      this.set('loading', true);
    },
  },

  _request(url) {
    this.set('displayResults', false);
    this.set('displayMessage', false);
    $.get(url)
      .then((result, statusText, xhr) => {
        this.set('loading', false);
        if (xhr.status === 200) {
          this._displayResults(result);
        } else {
          this._displayMessage(
            'This repository is pending evaluation. Please check back in a few minutes.',
          );
        }
      })
      .catch(() => {
        this.set('loading', false);
        this._displayMessage(
          'An error ocurred while trying to process your request.',
        );
      });
  },

  _displayResults(result) {
    this.set('displayResults', true);
    this.set('displayMessage', false);
    this.set('results', result);
  },

  _displayMessage(msg) {
    this.set('displayResults', false);
    this.set('displayMessage', true);
    this.set('message', msg);
  },
});
