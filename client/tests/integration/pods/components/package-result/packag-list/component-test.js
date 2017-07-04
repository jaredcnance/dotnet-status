import { moduleForComponent, test } from 'ember-qunit';
import hbs from 'htmlbars-inline-precompile';

moduleForComponent('package-result/packag-list', 'Integration | Component | package result/packag list', {
  integration: true
});

test('it renders', function(assert) {

  // Set any properties with this.set('myProperty', 'value');
  // Handle any actions with this.on('myAction', function(val) { ... });

  this.render(hbs`{{package-result/packag-list}}`);

  assert.equal(this.$().text().trim(), '');

  // Template block usage:
  this.render(hbs`
    {{#package-result/packag-list}}
      template block text
    {{/package-result/packag-list}}
  `);

  assert.equal(this.$().text().trim(), 'template block text');
});
