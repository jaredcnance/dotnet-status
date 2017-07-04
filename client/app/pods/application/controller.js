import Ember from 'ember';
import ENV from 'client/config/environment';

const { $ } = Ember;

export default Ember.Controller.extend({
  searchUrl: null,
  displayResults: false,
  results: null,

  actions: {
    search(url) {
      let hostUrl = `${ENV.APP.API_HOST}/${url}/`;
      this.set('searchUrl', hostUrl);
      this._request(hostUrl);
      this.set('loading', true);
    }
  },

  _request(url) {
    $.get(url).then((result) => {
        this.set('loading', false);
        this.set('results', result);
        this.set('displayResults', true);
    });
  }

});
