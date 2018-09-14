import Vue from 'vue'
import Vuex from 'vuex'

import navigation from './modules/navigation.js'
import selection from './modules/selection.js'
import blueprint from './modules/blueprint.js'
import auth from './modules/auth.js'
import user from './modules/user.js'
import filters from './modules/filters.js'
import entity from './modules/entity.js'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    navigation,
    selection,
    blueprint,
    auth,
    user,
    filters,
    entity
  }
})
