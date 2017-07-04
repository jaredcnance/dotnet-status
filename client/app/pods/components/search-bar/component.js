import Ember from 'ember';

export default Ember.Component.extend({
  user: 'research-institute',
  repo: 'json-api-dotnet-core',
  branch: 'master',
  path: 'src/JsonApiDotNetCore/JsonApiDotNetCore.csproj',

  actions: {
    search() {
      this.get('searchAction')(this._buildUrl());
    }
  },

  _buildUrl() {
    return `${this.get('user')}/${this.get('repo')}/${this.get('branch')}/${this.get('path')}`;
  }
});
