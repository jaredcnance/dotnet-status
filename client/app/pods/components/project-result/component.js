import Ember from 'ember';

const {
  computed,
  A
} = Ember;

export default Ember.Component.extend({
  tagName: '',

  upToDateFrameworks: computed('project', function () {
    let project = this.get('project');
    let upToDateFrameworks = A();
    project.frameworks.forEach((framework) => {
        let isUpToDate = framework.packages.filter((p) => {
            return p.isUpToDate === false;
        }).length === 0;
        upToDateFrameworks.pushObject(isUpToDate);
    });
    return upToDateFrameworks;
  }),

});
