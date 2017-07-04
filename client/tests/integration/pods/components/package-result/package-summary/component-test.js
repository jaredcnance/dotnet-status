import { moduleForComponent, test } from 'ember-qunit';
import hbs from 'htmlbars-inline-precompile';

moduleForComponent('package-result/package-summary', 'Integration | Component | package result/package summary', {
  integration: true
});

test('it renders', function(assert) {

  // Set any properties with this.set('myProperty', 'value');
  // Handle any actions with this.on('myAction', function(val) { ... });

  this.render(hbs`{{package-result/package-summary}}`);

  assert.equal(this.$().text().trim(), '');

  // Template block usage:
  this.render(hbs`
    {{#package-result/package-summary}}
      template block text
    {{/package-result/package-summary}}
  `);

  assert.equal(this.$().text().trim(), 'template block text');
});
