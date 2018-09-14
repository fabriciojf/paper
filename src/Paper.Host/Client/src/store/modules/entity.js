const state = {
  path: undefined,
  self: undefined,
  isDemoState: false
}

const getters = {
  self: state => state.self,
  entities: state => state.self.entities,
  path: state => state.path,
  isDemoState: state => state.isDemoState
}

const mutations = {

  setEntity: (state, data) => {
    state.self = data
  },

  setPath: (state, data) => {
    state.path = data + '?f=json'
  },

  setDemoState (state, data) {
    state.isDemoState = data
  }

}

export default {
  state,
  getters,
  mutations,
  namespaced: true
}
