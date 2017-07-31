import Ember from 'ember';

const {
  A,
} = Ember;

export default Ember.Component.extend({
  maximized: true,
  content: A(),
  actions: {
    toggle() {
      this.set('maximized', !this.get('maximized'));
    }
  }
});
