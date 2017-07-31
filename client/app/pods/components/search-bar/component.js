import Ember from 'ember';

export default Ember.Component.extend({
  repo: 'https://github.com/jaredcnance/dotnet-status.git',

  actions: {
    search() {
      this.get('searchAction')(this._buildUrl());
    }
  },

  _buildUrl() {
    return this.get('repo');
  }
});
