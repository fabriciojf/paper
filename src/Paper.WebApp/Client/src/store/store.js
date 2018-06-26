import Vue from 'vue'
import Vuex from 'vuex'

import navigation from './modules/navigation.js'
import selection from './modules/selection.js'
import blueprint from './modules/blueprint.js'
import auth from './modules/auth.js'
import user from './modules/user.js'
import filters from './modules/filters.js'

Vue.use(Vuex)

const state = {
  pathEntity: null,
  entity: null,
  isDemoState: false
}

const getters = {
  entity: state => state.entity
}

const mutations = {

  setEntity: (state, data) => {
    state.entity = data
  },

  setEntityPath: (state, data) => {
    state.pathEntity = data
  },

  setDemoState (state, data) {
    state.isDemoState = data
  }

}

export default new Vuex.Store({
  state,
  mutations,
  getters,
  modules: {
    navigation,
    selection,
    blueprint,
    auth,
    user,
    filters
  }
})
