import axios from 'axios'

const state = {
  token: localStorage.getItem('user-token') || '',
  status: ''
}

const getters = {
  isAuthenticated: state => !!state.token,
  authStatus: state => state.status
}

const mutations = {

  request (state) {
    state.status = 'loading'
  },

  success (state, token) {
    state.status = 'success'
    state.token = token
  },

  error (state) {
    state.status = 'error'
  },

  removeToken (state) {
    state.token = undefined
  }

}

const actions = {

  request ({ commit, dispatch }, user) {
    return new Promise((resolve, reject) => {
      commit('request')
      var loginUrl = 'http://localhost:3000/auth/login'
      axios({ url: loginUrl, data: user, method: 'POST' }).then(response => {
        var token = response.data.token
        localStorage.setItem('user-token', token)
        axios.defaults.headers.common['Authorization'] = token
        commit('success', token)
        // dispatch(USER_REQUEST)
        resolve(response)
      }).catch(err => {
        commit('error', err)
        localStorage.removeItem('user-token')
        reject(err)
      })
    })
  },

  logout ({commit, dispatch}) {
    return new Promise((resolve, reject) => {
      localStorage.removeItem('user-token')
      commit('removeToken')
      delete axios.defaults.headers.common['Authorization']
      resolve()
    })
  }

}

export default {
  state,
  getters,
  mutations,
  actions,
  namespaced: true
}
