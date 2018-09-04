import Parser from './Parser.js'
import Errors from './Errors.js'
import Demo from './Demo.js'
import Requester from './Requester.js'
import Page from './Page.js'
import Blueprint from './Blueprint.js'
import Actions from './Actions.js'
import Pagination from './Pagination.js'
import Navigation from './Navigation.js'
import Grid from './Grid.js'
import Auth from './Auth.js'
import State from './State.js'
import User from './User.js'
import Filters from './Filters.js'
import Data from './Data.js'
import Cards from './Cards.js'
import DataTypeEnum from './DataTypeEnum.js'
import TypeEnum from './TypeEnum.js'

const paper = {
  install (Vue, options) {
    var errors = new Errors()
    var parser = new Parser(options)
    var demo = new Demo(options, parser)
    var requester = new Requester(options, demo)
    var page = new Page(options, requester, parser, demo)
    var blueprint = new Blueprint(options, page, demo, requester)
    var actions = new Actions(options)
    var pagination = new Pagination(options, requester)
    var navigation = new Navigation(options, actions)
    var grid = new Grid(options)
    var auth = new Auth(options)
    var state = new State(options)
    var user = new User(options)
    var filters = new Filters(options, actions)
    var data = new Data(options)
    var dataType = new DataTypeEnum()
    var typeEnum = new TypeEnum()
    var cards = new Cards(options)

    var paper = {
      blueprint: blueprint,
      requester: requester,
      demo: demo,
      errors: errors,
      page: page,
      actions: actions,
      pagination: pagination,
      navigation: navigation,
      grid: grid,
      auth: auth,
      state: state,
      user: user,
      filters: filters,
      data: data,
      parser: parser,
      dataType: dataType,
      type: typeEnum,
      cards: cards,

      getEntity () {
        return options.store.getters.entity
      },

      isPaperPage (path) {
        if (path === null) {
          return false
        }
        var isPaperPage = path.match(/\/page/g) || path.match(/page/g)
        return isPaperPage
      },

      isFormPage (path) {
        if (path === null) {
          return false
        }
        var isFormPage = path.match(/\/form/g) || path.match(/form/g)
        return isFormPage
      },

      isDemoPage (path) {
        return demo.isDemoPage(path)
      },

      load (route) {
        this.blueprint.load().then(() => {
          if (this.page.isRoot() || this.demo.isRoot()) {
            this.blueprint.goToIndexPage()
            return
          }
          var validRoute = options.router.options.routes.find(route => route.name === route.name)
          var isPaperPage = this.isPaperPage(route.path)
          if (isPaperPage || !validRoute) {
            this.requester.redirectToPage(route.path)
            return
          }
          var isFormPage = this.isFormPage(route.name)
          if (isFormPage) {
            var action = route.query && route.query.action ? route.query.action : ''
            this.requester.redirectToForm(route.path, action)
          }
        })
      },

      setEntityPath (path) {
        path = 'http://localhost:5000' + path
        options.store.commit('setEntityPath', path)
      },

      setEntity (data) {
        options.store.commit('setEntity', data)
      }
    }

    Vue.prototype.$paper = paper
  }
}

export default paper
