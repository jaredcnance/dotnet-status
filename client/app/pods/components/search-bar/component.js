import Ember from 'ember';

export default Ember.Component.extend({
  user: 'research-institute',
  repo: 'json-api-dotnet-core',

  actions: {
    search() {
      this.get('searchAction')(this._buildUrl());
    }
  },

  _buildUrl() {
    return `${this.get('user')}/${this.get('repo')}`;
  }
});
