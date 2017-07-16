import Ember from 'ember';

const {
  computed
} = Ember;

export default Ember.Component.extend({
  tagName: '',

  isUpToDate: computed('framework', function () {
    let framework = this.get('framework');
    let isUpToDate = framework.packages.filter((p) => {
      return p.isUpToDate === false;
    }).length === 0;

    return isUpToDate;
  }),

});
